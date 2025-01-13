using System.Collections.Generic;
using System.Threading.Tasks;
using LearningClassLibary.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;
using LearningClassLibary.Models;
using LearningClassLibrary.Interfaces;
using LearningClassLibrary.Models;

namespace LearningPlatform.API.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly string _connectionString;

        public QuestionService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Get all questions
        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            var questions = new List<Question>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM questions";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                questions.Add(new Question
                {
                    QuestionID = reader.GetInt32("QuestionID"),
                    QuestionText = reader.GetString("QuestionText"),
                    CorrectAnswer = reader.GetString("CorrectAnswer"),
                    Difficulty = reader.IsDBNull("DifficultyLevel")
                        ? (DifficultyLevel?)null
                        : (DifficultyLevel?)reader.GetInt32("DifficultyLevel"),
                    Topic = reader.IsDBNull("Topic") ? null : reader.GetString("Topic"),
                    IncorrectAnswer1 = reader.IsDBNull("IncorrectAnswer1") ? null : reader.GetString("IncorrectAnswer1"),
                    IncorrectAnswer2 = reader.IsDBNull("IncorrectAnswer2") ? null : reader.GetString("IncorrectAnswer2"),
                    IncorrectAnswer3 = reader.IsDBNull("IncorrectAnswer3") ? null : reader.GetString("IncorrectAnswer3")
                });
            }

            return questions;
        }

        // Get question count

        public async Task<int> GetQuestionsCountAsync()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                var query = "SELECT COUNT(*) FROM questions";
                using var command = new MySqlCommand(query, connection);
                var result = await command.ExecuteScalarAsync();
                if (result != null && result is long count)
                {
                    return (int)count;
                }
                else
                {
                    // טיפול במקרה של שגיאה או null
                    return 0; // או זרוק exception
                }
            }
            catch (Exception ex)
            {
                // טיפול בשגיאה
                Console.WriteLine($"Error in GetQuestionsCountAsync: {ex.Message}");
                return 0; // או זרוק exception
            }
        }
        // Get question by ID
        public async Task<Question?> GetQuestionByIdAsync(int questionId)
        {
            Question? question = null;
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "SELECT * FROM questions WHERE QuestionID = @QuestionID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@QuestionID", questionId);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                question = new Question
                {
                    QuestionID = reader.GetInt32("QuestionID"),
                    QuestionText = reader.GetString("QuestionText"),
                    CorrectAnswer = reader.GetString("CorrectAnswer"),
                    Difficulty = reader.IsDBNull("DifficultyLevel")
                        ? (DifficultyLevel?)null
                        : (DifficultyLevel)reader.GetInt32("DifficultyLevel"),
                    Topic = reader.IsDBNull("Topic") ? null : reader.GetString("Topic"),
                    IncorrectAnswer1 = reader.IsDBNull("IncorrectAnswer1") ? null : reader.GetString("IncorrectAnswer1"),
                    IncorrectAnswer2 = reader.IsDBNull("IncorrectAnswer2") ? null : reader.GetString("IncorrectAnswer2"),
                    IncorrectAnswer3 = reader.IsDBNull("IncorrectAnswer3") ? null : reader.GetString("IncorrectAnswer3")
                };
            }

            return question;
        }

        // Add a new question
        public async Task AddQuestionAsync(Question question)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "INSERT INTO questions (QuestionText, CorrectAnswer, DifficultyLevel, Topic, IncorrectAnswer1, IncorrectAnswer2, IncorrectAnswer3) VALUES (@QuestionText, @CorrectAnswer, @DifficultyLevel, @Topic, @IncorrectAnswer1, @IncorrectAnswer2, @IncorrectAnswer3)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@QuestionText", question.QuestionText);
            command.Parameters.AddWithValue("@CorrectAnswer", question.CorrectAnswer);
            command.Parameters.AddWithValue("@DifficultyLevel", (object?)question.Difficulty ?? DBNull.Value);
            command.Parameters.AddWithValue("@Topic", (object?)question.Topic ?? DBNull.Value);
            command.Parameters.AddWithValue("@IncorrectAnswer1", (object?)question.IncorrectAnswer1 ?? DBNull.Value);
            command.Parameters.AddWithValue("@IncorrectAnswer2", (object?)question.IncorrectAnswer2 ?? DBNull.Value);
            command.Parameters.AddWithValue("@IncorrectAnswer3", (object?)question.IncorrectAnswer3 ?? DBNull.Value);
            await command.ExecuteNonQueryAsync();
        }

        // Update an existing question
        public async Task UpdateQuestionAsync(Question question)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "UPDATE questions SET QuestionText = @QuestionText, CorrectAnswer = @CorrectAnswer, DifficultyLevel = @DifficultyLevel, Topic = @Topic, IncorrectAnswer1 = @IncorrectAnswer1, IncorrectAnswer2 = @IncorrectAnswer2, IncorrectAnswer3 = @IncorrectAnswer3 WHERE QuestionID = @QuestionID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@QuestionID", question.QuestionID);
            command.Parameters.AddWithValue("@QuestionText", question.QuestionText);
            command.Parameters.AddWithValue("@CorrectAnswer", question.CorrectAnswer);
            command.Parameters.AddWithValue("@DifficultyLevel", (object?)question.Difficulty ?? DBNull.Value);
            command.Parameters.AddWithValue("@Topic", (object?)question.Topic ?? DBNull.Value);
            command.Parameters.AddWithValue("@IncorrectAnswer1", (object?)question.IncorrectAnswer1 ?? DBNull.Value);
            command.Parameters.AddWithValue("@IncorrectAnswer2", (object?)question.IncorrectAnswer2 ?? DBNull.Value);
            command.Parameters.AddWithValue("@IncorrectAnswer3", (object?)question.IncorrectAnswer3 ?? DBNull.Value);
            await command.ExecuteNonQueryAsync();
        }

        // Delete a question
        public async Task DeleteQuestionAsync(int questionId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = "DELETE FROM questions WHERE QuestionID = @QuestionID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@QuestionID", questionId);
            await command.ExecuteNonQueryAsync();
        }
    }
}
