using System.Collections.Generic;
using System.Threading.Tasks;
using LearningClassLibary.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;
using LearningClassLibary.Models;

namespace LearningPlatform.API.Services
{
    public class PersonService : IPersonService
    {
        private readonly string _connectionString;

        public PersonService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Get all persons
        public async Task<List<Person>> GetAllPersonsAsync()
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
                    Role = Enum.TryParse<RoleIn>(reader.GetString("Role"), out var role) ? role : RoleIn.Student // Safely parse Role
                });
            }
            return persons;
        }

        // Get person by ID
        public async Task<Person?> GetPersonByIdAsync(int PersonID)
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
                    Role = Enum.TryParse<RoleIn>(reader.GetString("Role"), out var role) ? role : RoleIn.Student // Safely parse Role
                };
            }
            return person;
        }

        // Add a new person
        public async Task AddPersonAsync(Person person)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "INSERT INTO Person (FirstName, LastName, Email, PhoneNumber, DateOfBirth, Role) VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth, @Role)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@FirstName", person.FirstName);
            command.Parameters.AddWithValue("@LastName", person.LastName);
            command.Parameters.AddWithValue("@Email", person.Email);
            command.Parameters.AddWithValue("@PhoneNumber", person.PhoneNumber);
            command.Parameters.AddWithValue("@DateOfBirth", person.DateOfBirth);
            command.Parameters.AddWithValue("@Role", person.Role.ToString()); // Store as string in database
            await command.ExecuteNonQueryAsync();
        }

        // Update an existing person
        public async Task UpdatePersonAsync(Person person)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "UPDATE Person SET FirstName = @FirstName, LastName = @LastName, Email = @Email, PhoneNumber = @PhoneNumber, DateOfBirth = @DateOfBirth, Role = @Role WHERE PersonID = @PersonID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", person.PersonID);
            command.Parameters.AddWithValue("@FirstName", person.FirstName);
            command.Parameters.AddWithValue("@LastName", person.LastName);
            command.Parameters.AddWithValue("@Email", person.Email);
            command.Parameters.AddWithValue("@PhoneNumber", person.PhoneNumber);
            command.Parameters.AddWithValue("@DateOfBirth", person.DateOfBirth);
            command.Parameters.AddWithValue("@Role", person.Role.ToString()); // Store as string in database
            await command.ExecuteNonQueryAsync();
        }

        // Delete a person
        public async Task DeletePersonAsync(int PersonID)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "DELETE FROM Person WHERE PersonID = @PersonID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);
            await command.ExecuteNonQueryAsync();
        }
    }
}
