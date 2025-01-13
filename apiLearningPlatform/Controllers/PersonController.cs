using Microsoft.AspNetCore.Mvc;
using LearningClassLibary.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using LearningClassLibrary.Models;

namespace LearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly string _connectionString;

        public PersonController(IConfiguration configuration)
        {
            // Get the connection string from appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: api/Person
        [HttpGet]
        public async Task<IActionResult> GetAllPersons()
        {
            var persons = new List<Person>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM Person";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                persons.Add(new Person
                {
                    PersonID = reader.GetInt32("PersonID"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Email = reader.GetString("Email"),
                    PhoneNumber = reader.GetString("PhoneNumber"),
                    DateOfBirth = reader.GetDateTime("DateOfBirth"),
                    Role = Enum.TryParse<RoleIn>(reader.GetString("Role"), out var role) ? role : RoleIn.Student
                });
            }
            return Ok(persons);
        }

        // GET: api/Person/{PersonID}
        [HttpGet("{PersonID}")]
        public async Task<IActionResult> GetPersonById(int PersonID)
        {
            Person? person = null;
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM Person WHERE PersonID = @PersonID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                person = new Person
                {
                    PersonID = reader.GetInt32("PersonID"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Email = reader.GetString("Email"),
                    PhoneNumber = reader.GetString("PhoneNumber"),
                    DateOfBirth = reader.GetDateTime("DateOfBirth"),
                    Role = Enum.TryParse<RoleIn>(reader.GetString("Role"), out var role) ? role : RoleIn.Student
                };
            }
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person);
        }

        // POST: api/Person
        [HttpPost]
        public async Task<IActionResult> AddPerson([FromBody] Person person)
        {
          

            if (person == null)
            {
                return BadRequest("Person is null.");
            }

            // Hash the password before storing
            person.Password = HashPassword(person.Password);

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "INSERT INTO Person (FirstName, LastName, Email, PhoneNumber, DateOfBirth, Role, Password) " +
                        "VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth, @Role, @Password)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@FirstName", person.FirstName);
            command.Parameters.AddWithValue("@LastName", person.LastName);
            command.Parameters.AddWithValue("@Email", person.Email);
            command.Parameters.AddWithValue("@PhoneNumber", person.PhoneNumber);
            command.Parameters.AddWithValue("@DateOfBirth", person.DateOfBirth);
            command.Parameters.AddWithValue("@Role", person.Role.ToString());
            command.Parameters.AddWithValue("@Password", person.Password);
            await command.ExecuteNonQueryAsync();
            return CreatedAtAction(nameof(GetPersonById), new { PersonID = person.PersonID }, person);
        }

        // POST: api/Person/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (registerModel == null || string.IsNullOrWhiteSpace(registerModel.Password))
            {
                return BadRequest("Invalid registration data.");
            }

            var hashedPassword = HashPassword(registerModel.Password);

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "INSERT INTO Person (FirstName, LastName, Email, Password) VALUES (@FirstName, @LastName, @Email, @Password)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@FirstName", registerModel.FirstName);
            command.Parameters.AddWithValue("@LastName", registerModel.LastName);
            command.Parameters.AddWithValue("@Email", registerModel.Email);
            command.Parameters.AddWithValue("@Password", hashedPassword);
            await command.ExecuteNonQueryAsync();
            return Ok("Registration successful.");
        }

        // POST: api/Person/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Email or password is missing.");
            }

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Person WHERE Email = @Email";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", loginRequest.Email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var storedHash = reader.GetString("Password");
                if (!VerifyPassword(loginRequest.Password, storedHash))
                {
                    return Unauthorized("Invalid email or password.");
                }

                return Ok(new
                {
                    Message = "Login successful",
                    PersonID = reader.GetInt32("PersonID"),
                    Name = $"{reader.GetString("FirstName")} {reader.GetString("LastName")}",
                    Email = reader.GetString("Email"),
                    Role = reader.GetString("Role")
                });
            }
            return Unauthorized("Invalid email or password.");
        }

        // GET: api/Person/non-admin-users
        [HttpGet("non-admin-users")]
        public async Task<IActionResult> GetNonAdminUsers()
        {
            var nonAdminUsers = new List<Person>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM Person WHERE Role != @Role";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Role", "Admin");
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                nonAdminUsers.Add(new Person
                {
                    PersonID = reader.GetInt32("PersonID"),
                    FirstName = !reader.IsDBNull(reader.GetOrdinal("FirstName")) ? reader.GetString("FirstName") : string.Empty,
                    LastName = !reader.IsDBNull(reader.GetOrdinal("LastName")) ? reader.GetString("LastName") : string.Empty,
                    Email = !reader.IsDBNull(reader.GetOrdinal("Email")) ? reader.GetString("Email") : string.Empty,
                    PhoneNumber = !reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? reader.GetString("PhoneNumber") : string.Empty,
                    DateOfBirth = !reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? (DateTime?)reader.GetDateTime("DateOfBirth") : null,
                    Role = Enum.TryParse<RoleIn>(reader.GetString("Role"), out var role) ? role : RoleIn.Student
                });
            }
            return Ok(nonAdminUsers);
        }

        // PUT: api/Person/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] Person updatedPerson)
        {
            // Validate input.
            Console.WriteLine($"Updating Person: ID={updatedPerson.PersonID}, FirstName={updatedPerson.FirstName}, LastName={updatedPerson.LastName}, Role={updatedPerson.Role}");
            if (updatedPerson == null)
            {
                return BadRequest("Null person provided.");
            }

            // Ensure the PersonID from the URL is used (ignoring any value in the body).
            updatedPerson.PersonID = id;

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            // Retrieve the existing Email and PhoneNumber from the database
            // so that these fields cannot be modified via the update.
            var getExistingQuery = "SELECT Email, PhoneNumber FROM Person WHERE PersonID = @PersonID";
            string existingEmail;
            string existingPhoneNumber;

            using (var getCommand = new MySqlCommand(getExistingQuery, connection))
            {
                getCommand.Parameters.AddWithValue("@PersonID", id);
                using var reader = await getCommand.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    existingEmail = reader["Email"].ToString();
                    existingPhoneNumber = reader["PhoneNumber"].ToString();
                }
                else
                {
                    return NotFound();
                }
            }

            // Build the update query to update only the allowed fields:
            // FirstName, LastName, DateOfBirth, and Role.
            var updateQuery = @"
                UPDATE Person 
                SET FirstName = @FirstName, 
                    LastName = @LastName, 
                    DateOfBirth = @DateOfBirth, 
                    Role = @Role
                WHERE PersonID = @PersonID";

            using var command = new MySqlCommand(updateQuery, connection);
            command.Parameters.AddWithValue("@FirstName", updatedPerson.FirstName);
            command.Parameters.AddWithValue("@LastName", updatedPerson.LastName);

            // Handle nullable DateOfBirth.
            if (updatedPerson.DateOfBirth.HasValue)
            {
                command.Parameters.AddWithValue("@DateOfBirth", updatedPerson.DateOfBirth.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);
            }

            // Convert the Role enum to its string representation.
            command.Parameters.AddWithValue("@Role", updatedPerson.Role.ToString());
            command.Parameters.AddWithValue("@PersonID", id);

            // Execute the update command.
            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                // Reassign the original Email and PhoneNumber.
                updatedPerson.Email = existingEmail;
                updatedPerson.PhoneNumber = existingPhoneNumber;
                return Ok(updatedPerson);
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE: api/Person/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                // Delete related records from grouplist first.
                var deleteGroupListQuery = "DELETE FROM grouplist WHERE TeacherID = @PersonID";
                using var deleteGroupListCommand = new MySqlCommand(deleteGroupListQuery, connection, transaction);
                deleteGroupListCommand.Parameters.AddWithValue("@PersonID", id);
                await deleteGroupListCommand.ExecuteNonQueryAsync();

                // Delete the person record.
                var deletePersonQuery = "DELETE FROM Person WHERE PersonID = @PersonID";
                using var deletePersonCommand = new MySqlCommand(deletePersonQuery, connection, transaction);
                deletePersonCommand.Parameters.AddWithValue("@PersonID", id);
                var rowsAffected = await deletePersonCommand.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    await transaction.CommitAsync();
                    return NoContent();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error during delete operation: {ex.Message}");
                throw;
            }
        }

        // GET: api/Person/students-count
        [HttpGet("students-count")]
        public async Task<IActionResult> GetStudentsCount()
        {
            int studentsCount = 0;
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "SELECT COUNT(*) FROM Person WHERE Role = @Role";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Role", "Student");
            studentsCount = Convert.ToInt32(await command.ExecuteScalarAsync());
            return Ok(new { StudentsCount = studentsCount });
        }

        // Function to hash a password with a salt.
        private string HashPassword(string password)
        {
            // Create a random byte array for the salt.
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            // Hash the password using PBKDF2 with the generated salt.
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            // Combine the salt and hash for storage.
            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        // Function to verify a password against the stored hash.
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var parts = storedHash.Split(':');
            var salt = Convert.FromBase64String(parts[0]);
            var storedHashedPassword = parts[1];
            // Recreate the hash using the entered password and stored salt.
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed == storedHashedPassword;
        }
    }
}
