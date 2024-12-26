using Microsoft.AspNetCore.Mvc;
using LearningClassLibary.Interfaces;
using LearningClassLibary.Models;
using LearningClassLibrary.Interfaces;
using LearningClassLibrary.Models;

namespace LearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        // Get all questions
        [HttpGet]
        public async Task<IActionResult> GetAllQuestions()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Ok(questions);
        }

        // Get question by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return Ok(question);
        }

        // Add a new question
        [HttpPost]
        public async Task<IActionResult> AddQuestion([FromBody] Question question)
        {
            if (question == null)
            {
                return BadRequest("Question is null.");
            }
            await _questionService.AddQuestionAsync(question);
            return CreatedAtAction(nameof(GetQuestionById), new { id = question.QuestionID }, question);
        }

        // Update an existing question
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] Question question)
        {
            if (question == null || question.QuestionID != id)
            {
                return BadRequest("Invalid question data.");
            }
            await _questionService.UpdateQuestionAsync(question);
            return NoContent();
        }

        // Delete a question
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            await _questionService.DeleteQuestionAsync(id);
            return NoContent();
        }
    }
}
