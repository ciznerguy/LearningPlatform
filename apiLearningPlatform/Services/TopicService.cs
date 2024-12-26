
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LearningClassLibrary.Interfaces;
using LearningClassLibrary.Models;
using MySql.Data.MySqlClient;

namespace LearningClassLibrary.Services
{
    public class TopicService : ITopicService
    {
        private readonly string _connectionString;

        public TopicService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Topic>> GetAllTopicsAsync()
        {
            var topics = new List<Topic>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT TopicID, TopicName, Description FROM topics";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        topics.Add(new Topic
                        {
                            TopicID = reader.GetInt32("TopicID"),
                            TopicName = reader.GetString("TopicName"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description")
                        });
                    }
                }
            }
            return topics;
        }

        public async Task<Topic?> GetTopicByIdAsync(int topicId)
        {
            Topic? topic = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT TopicID, TopicName, Description FROM topics WHERE TopicID = @TopicID";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TopicID", topicId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            topic = new Topic
                            {
                                TopicID = reader.GetInt32("TopicID"),
                                TopicName = reader.GetString("TopicName"),
                                Description = reader.IsDBNull("Description") ? null : reader.GetString("Description")
                            };
                        }
                    }
                }
            }
            return topic;
        }

        public async Task AddTopicAsync(Topic topic)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "INSERT INTO topics (TopicName, Description) VALUES (@TopicName, @Description)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TopicName", topic.TopicName);
                    command.Parameters.AddWithValue("@Description", topic.Description ?? (object)DBNull.Value);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateTopicAsync(Topic topic)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE topics SET TopicName = @TopicName, Description = @Description WHERE TopicID = @TopicID";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TopicName", topic.TopicName);
                    command.Parameters.AddWithValue("@Description", topic.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@TopicID", topic.TopicID);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteTopicAsync(int topicId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "DELETE FROM topics WHERE TopicID = @TopicID";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TopicID", topicId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
