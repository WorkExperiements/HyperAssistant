using HyperBuddy.Prompter.Models;
using HyperBuddy.Prompter.Services;
using Microsoft.AspNetCore.Mvc;

namespace HyperBuddy.Prompter;

public class PromptController(IPromptingService promptsvc, ILearningService learningService) : Controller
{
    private readonly IPromptingService _promptingService = promptsvc;
    private readonly ILearningService _learningService = learningService;

    [HttpGet]
    public IActionResult Index(PromptRequest promptRequest)
    {
        // make sure the chat has history
        if(!(promptRequest?.History?.Any() ?? false))
        {
            return BadRequest("There are no items in the chat history");
        }
        var response = _promptingService.PromptCogSearch(promptRequest.History);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Learn(LearnRequest learnRequest)
    {
        var learnResult = await _learningService.LearnFromUrl(learnRequest.Url ?? string.Empty, learnRequest.Name ?? string.Empty);
        return Ok(learnResult);
    }
}

