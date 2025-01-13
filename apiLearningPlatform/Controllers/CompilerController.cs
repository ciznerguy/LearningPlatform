using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LearningClassLibrary.Services;

namespace apiLearningPlatform.Controllers
{
    [ApiController]
    [Route("api/compiler")]
    public class CompilerController : ControllerBase
    {
        private readonly CodeExecutionService _codeService;

        public CompilerController(CodeExecutionService codeService)
        {
            _codeService = codeService;
        }

        [HttpPost]
        public async Task<IActionResult> RunCode([FromBody] CodeRequest request)
        {
            string result = await _codeService.ExecuteUserCode(request.Code);

            // ✅ הדפסת הפלט שמוחזר ישירות לטרמינל של השרת (Swagger)
            Console.WriteLine($"[API Output]: {result}");

            return Ok(new { Output = result });
        }
    }

    public class CodeRequest
    {
        public string Code { get; set; }
    }
}
