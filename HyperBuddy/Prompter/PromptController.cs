using Microsoft.AspNetCore.Mvc;

namespace HyperBuddy.Prompter
{
    public class PromptController(IPromptingService promptsvc) : Controller
    {
        private readonly IPromptingService _promptingService = promptsvc;

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(_promptingService.PromptCogSearch("Tell me about New York"));
        }
    }
}
