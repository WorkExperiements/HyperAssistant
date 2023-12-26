using HyperBuddy.Prompter.Models;
using HyperBuddy.Prompter.Services;
using Microsoft.AspNetCore.Mvc;

namespace HyperBuddy.Prompter;

public class PromptController(IPromptingService promptsvc, ILearningService learningService) : Controller
{
    private readonly IPromptingService _promptingService = promptsvc;
    private readonly ILearningService _learningService = learningService;

    [HttpGet]
    public IActionResult Index()
    {
        return Ok(_promptingService.PromptCogSearch("Tell me about New York"));
    }

    [HttpPost]
    public async Task<IActionResult> Learn(LearnRequest learnRequest)
    {
        var learnResult = await _learningService.LearnFromUrl(learnRequest.Url ?? string.Empty, learnRequest.Name ?? string.Empty);
        return Ok(learnResult);
    }
}

