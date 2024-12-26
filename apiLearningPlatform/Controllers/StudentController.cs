using Microsoft.AspNetCore.Mvc;

namespace apiLearningPlatform.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
