using Microsoft.AspNetCore.Mvc;
using LearningClassLibrary.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using LearningClassLibary.Models;
using System.Data;

namespace LearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionExamController : ControllerBase
    {
        private readonly string _connectionString;

        public QuestionExamController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // 🔹 שליפת כל השאלות
        [HttpGet]
        public async Task<IActionResult> GetAllQuestions()
        {
            var questions = new List<QuestionExam>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "SELECT QuestionID, QuestionText, ExpectedOutput, TopicID, DifficultyLevel FROM questions";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                questions.Add(new QuestionExam(
                    reader.GetInt32("QuestionID"),
                    reader.GetString("QuestionText"),
                    reader.GetString("ExpectedOutput"),
                    reader.GetInt32("TopicID"),
                    reader.GetInt32("DifficultyLevel")
                ));
            }
            return Ok(questions);
        }

        // 🔹 הוספת שאלה חדשה
        [HttpPost]
        public async Task<IActionResult> AddQuestion([FromBody] QuestionExam question)
        {
            if (question == null)
                return BadRequest("Question is null.");

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "INSERT INTO questions (QuestionText, ExpectedOutput, TopicID, DifficultyLevel) VALUES (@QuestionText, @ExpectedOutput, @TopicID, @DifficultyLevel)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@QuestionText", question.QuestionText);
            command.Parameters.AddWithValue("@ExpectedOutput", question.ExpectedOutput);
            command.Parameters.AddWithValue("@TopicID", question.TopicID);
            command.Parameters.AddWithValue("@DifficultyLevel", question.DifficultyLevel);
            await command.ExecuteNonQueryAsync();
            return Ok("Question added successfully.");
        }

        // 🔹 עדכון שאלה קיימת
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionExam question)
        {
            if (question == null)
                return BadRequest("Invalid question data.");

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "UPDATE questions SET QuestionText = @QuestionText, ExpectedOutput = @ExpectedOutput, TopicID = @TopicID, DifficultyLevel = @DifficultyLevel WHERE QuestionID = @QuestionID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@QuestionText", question.QuestionText);
            command.Parameters.AddWithValue("@ExpectedOutput", question.ExpectedOutput);
            command.Parameters.AddWithValue("@TopicID", question.TopicID);
            command.Parameters.AddWithValue("@DifficultyLevel", question.DifficultyLevel);
            command.Parameters.AddWithValue("@QuestionID", id);
            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
                return Ok("Question updated successfully.");
            return NotFound("Question not found.");
        }

        // 🔹 מחיקת שאלה לפי מזהה
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "DELETE FROM questions WHERE QuestionID = @QuestionID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@QuestionID", id);
            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
                return Ok("Question deleted successfully.");
            return NotFound("Question not found.");
        }
    }
}
