using Microsoft.AspNetCore.Mvc;
using LearningClassLibary.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;


namespace LearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly string _connectionString;

        public PersonsController(IConfiguration configuration)
        {
            // מקבל את מחרוזת החיבור מ-appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Get all persons
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

        // Get person by ID
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

        [HttpPost]
        public async Task<IActionResult> AddPerson([FromBody] Person person)
        {
            if (person == null)
            {
                return BadRequest("Person is null.");
            }

            person.Password = HashPassword(person.Password); // Hash the password

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "INSERT INTO Person (FirstName, LastName, Email, PhoneNumber, DateOfBirth, Role, Password) VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth, @Role, @Password)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@FirstName", person.FirstName);
            command.Parameters.AddWithValue("@LastName", person.LastName);
            command.Parameters.AddWithValue("@Email", person.Email);
            command.Parameters.AddWithValue("@PhoneNumber", person.PhoneNumber);
            command.Parameters.AddWithValue("@DateOfBirth", person.DateOfBirth);
            command.Parameters.AddWithValue("@Role", person.Role.ToString());
            command.Parameters.AddWithValue("@Password", person.Password); // Store hashed password
            await command.ExecuteNonQueryAsync();
            return CreatedAtAction(nameof(GetPersonById), new { PersonID = person.PersonID }, person);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (registerModel == null || string.IsNullOrWhiteSpace(registerModel.Password))
            {
                return BadRequest("Invalid registration data.");
            }

            var hashedPassword = HashPassword(registerModel.Password); // Hash the password

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "INSERT INTO Person (FirstName, LastName, Email, Password) VALUES (@FirstName, @LastName, @Email, @Password)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@FirstName", registerModel.FirstName);
            command.Parameters.AddWithValue("@LastName", registerModel.LastName);
            command.Parameters.AddWithValue("@Email", registerModel.Email);
            command.Parameters.AddWithValue("@Password", hashedPassword); // Store hashed password
            await command.ExecuteNonQueryAsync();
            return Ok("Registration successful.");
        }


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
                var storedHash = reader.GetString("Password"); // Retrieve hashed password

                if (!VerifyPassword(loginRequest.Password, storedHash)) // Verify entered password
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

        // Function to hash a password with a salt
        private string HashPassword(string password)
        {
            // Create a random byte array to use as a salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt); // Fill the salt with random bytes
            }

            // Hash the password using PBKDF2 with the generated salt
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,              // Original password
                salt: salt,                      // Randomly generated salt
                prf: KeyDerivationPrf.HMACSHA256, // Hashing algorithm
                iterationCount: 10000,           // Number of iterations
                numBytesRequested: 256 / 8));    // Length of the derived key

            // Combine the salt and the hash into a single string for storage
            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        // Function to verify a password against the stored hash
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Split the stored hash into its salt and hash components
            var parts = storedHash.Split(':');
            var salt = Convert.FromBase64String(parts[0]); // Extract the stored salt
            var storedHashedPassword = parts[1];          // Extract the stored hash

            // Recreate the hash from the entered password and the stored salt
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,         // Password entered by the user
                salt: salt,                        // Stored salt
                prf: KeyDerivationPrf.HMACSHA256,  // Same hashing algorithm
                iterationCount: 10000,             // Same number of iterations
                numBytesRequested: 256 / 8));      // Same derived key length

            // Compare the recreated hash with the stored hash
            return hashed == storedHashedPassword;
        }

        // Explanation:
        // The `HashPassword` method generates a secure hash for a given password using a random salt.
        // It combines the salt and the hash in a single string for storage in a database.
        // The `VerifyPassword` method validates a user-provided password by extracting the salt and 
        // stored hash, recreating the hash with the provided password, and comparing the two.
        // This approach ensures secure storage of passwords, protecting against attacks like rainbow tables. 




    }
}
