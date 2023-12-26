using HtmlAgilityPack;
using HyperBuddy.Prompter.Models;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace HyperBuddy.Prompter.Services;

public interface ILearningService
{
    Task<LearnResponse> LearnFromUrl(string url, string recipeName);
}
public class LearningService(IHttpClientFactory httpClientFactory, IBlobStorageService blobStorageService, IAzureSearchService azureSearchService) : ILearningService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IBlobStorageService _blobStorageService = blobStorageService;
    private readonly IAzureSearchService _azureSearchService = azureSearchService;

    public async Task<LearnResponse> LearnFromUrl(string url, string recipeName)
    {

        var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.GetAsync(url);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var strContent = await httpResponseMessage.Content.ReadAsStringAsync();
            var cleanContent = StripAndCleanHtml(strContent);

            if (cleanContent == null)
            {
                return new() { IsSuccess = false, Message = "Html content cannot be parsed correctly." };
            }

            // (optional) clean up directory.

            // save contents into a file in blob
            await _blobStorageService.UploadFile(cleanContent, recipeName);

            // re-index directory
            await _azureSearchService.RunRecipeIndexer();
        }
        else
        {
            return new() { IsSuccess = false, Message = "Cannot retrieve from URL." };
        }
        return new() { IsSuccess = true, Message = string.Empty };
    }

    private static string? StripAndCleanHtml(string origHtml)
    {
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(origHtml);

        if (htmlDoc == null)
            return null;

        var strippedHtml = htmlDoc.DocumentNode.InnerText;

        var collapsedReturns = Regex.Replace(strippedHtml, @"[\r\n]+", "\r\n");
        var collapsedReturnsAndSpaces = Regex.Replace(collapsedReturns, @"(\r\n\s+)+", "\r\n ");
        var decoded = collapsedReturnsAndSpaces.Replace("&nbsp;", " ");

        return decoded;
    }
}

