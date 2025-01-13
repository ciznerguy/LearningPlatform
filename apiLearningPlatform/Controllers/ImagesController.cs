using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace apiLearningPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        // Data Transfer Object (DTO) for image representation.
        // The ImageData property (byte array) is automatically serialized as a Base64 string in JSON.
        public class ImageDto
        {
            public int ImageId { get; set; } // Auto-generated ID for the image (used in responses).
            public int OwnerId { get; set; } // User ID of the uploader.
            public byte[] ImageData { get; set; } // Image stored as byte array.
            public string ImageType { get; set; } // MIME type (e.g., "image/png", "image/jpeg").
        }

        // Injected configuration to access database connection string from appsettings.json.
        private readonly IConfiguration _configuration;

        public ImagesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 🔹 POST Endpoint: Upload an image
        // This endpoint expects JSON input with ImageData (byte array), OwnerId, and ImageType.
        [HttpPost("upload")]
        public IActionResult UploadImage([FromBody] ImageDto request)
        {
            Console.WriteLine("🔹 Starting image upload process...");

            // Ensure image data is provided
            if (request.ImageData == null || request.ImageData.Length == 0)
            {
                Console.WriteLine("❌ Error: No image data provided.");
                return BadRequest("No image data provided.");
            }

            Console.WriteLine($"📂 Received image from OwnerID {request.OwnerId}, size: {request.ImageData.Length} bytes, type: {request.ImageType}");

            try
            {
                // Retrieve connection string from appsettings.json
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    Console.WriteLine("🔗 Database connection opened successfully.");

                    // Insert the image record into the `images` table
                    string query = "INSERT INTO images (OwnerID, ImageData, ImageType) VALUES (@OwnerID, @ImageData, @ImageType)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OwnerID", request.OwnerId);
                        cmd.Parameters.AddWithValue("@ImageData", request.ImageData);
                        cmd.Parameters.AddWithValue("@ImageType", request.ImageType);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"📥 {rowsAffected} row(s) inserted into the images table.");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"❌ Database error: {ex.Message}");
                return StatusCode(500, "Database connection error. Please verify your credentials and permissions.");
            }

            Console.WriteLine("🎉 Image saved successfully to the database!");
            return Ok("Image saved successfully.");
        }

        // 🔹 GET Endpoint: Retrieve an image by ID
        // Returns an ImageDto object with ImageId, OwnerId, ImageData (Base64 encoded), and ImageType.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            Console.WriteLine($"🔹 Retrieving image with ID: {id}");

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    Console.WriteLine("🔗 Database connection opened successfully for retrieval.");

                    // Retrieve the image data from the database
                    string query = "SELECT ImageID, OwnerID, ImageData, ImageType FROM images WHERE ImageID = @id LIMIT 1";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var imageDto = new ImageDto
                                {
                                    ImageId = Convert.ToInt32(reader["ImageID"]),
                                    OwnerId = Convert.ToInt32(reader["OwnerID"]),
                                    ImageData = (byte[])reader["ImageData"],
                                    ImageType = reader["ImageType"].ToString()
                                };
                                Console.WriteLine("✅ Image retrieved successfully.");
                                return Ok(imageDto);
                            }
                            else
                            {
                                Console.WriteLine("❌ No image found with the given ID.");
                                return NotFound("Image not found.");
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"❌ Database error during retrieval: {ex.Message}");
                return StatusCode(500, "Database error during image retrieval.");
            }
        }
    }
}
