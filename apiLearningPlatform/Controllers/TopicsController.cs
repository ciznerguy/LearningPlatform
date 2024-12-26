using Microsoft.AspNetCore.Mvc;
using LearningClassLibrary.Interfaces;
using LearningClassLibrary.Models;

namespace LearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicsController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        // Get all topics
        [HttpGet]
        public async Task<IActionResult> GetAllTopics()
        {
            var topics = await _topicService.GetAllTopicsAsync();
            return Ok(topics);
        }

        // Get topic by ID
        [HttpGet("{TopicID}")]
        public async Task<IActionResult> GetTopicById(int TopicID)
        {
            var topic = await _topicService.GetTopicByIdAsync(TopicID);
            if (topic == null)
            {
                return NotFound();
            }
            return Ok(topic);
        }

        // Add a new topic
        [HttpPost]
        public async Task<IActionResult> AddTopic([FromBody] Topic topic)
        {
            if (topic == null)
            {
                return BadRequest("Topic data is null.");
            }

            await _topicService.AddTopicAsync(topic);
            return CreatedAtAction(nameof(GetTopicById), new { TopicID = topic.TopicID }, topic);
        }

        // Update an existing topic
        [HttpPut("{TopicID}")]
        public async Task<IActionResult> UpdateTopic(int TopicID, [FromBody] Topic topic)
        {
            if (topic == null)
            {
                return BadRequest("Topic data is null.");
            }

            // Assign TopicID from URL
            topic.TopicID = TopicID;

            await _topicService.UpdateTopicAsync(topic);
            return NoContent();
        }

        // Delete a topic
        [HttpDelete("{TopicID}")]
        public async Task<IActionResult> DeleteTopic(int TopicID)
        {
            var topic = await _topicService.GetTopicByIdAsync(TopicID);
            if (topic == null)
            {
                return NotFound();
            }

            await _topicService.DeleteTopicAsync(TopicID);
            return NoContent();
        }
    }
}
