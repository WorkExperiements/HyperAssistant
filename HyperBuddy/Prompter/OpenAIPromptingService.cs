using Microsoft.Extensions.Options;

namespace HyperBuddy.Prompter;

public interface IPromptingService
{
    string GetEndpointConfig();
}
public class OpenAIPromptingService : IPromptingService
{
    private readonly string _oaiEndpoint;
    private readonly string _oaiKey;
    private readonly string _oaiModelName;
    private readonly string _azureSearchEndpoint;
    private readonly string _azureSearchKey;
    private readonly string _azureSearchIndex;
    public OpenAIPromptingService(IConfiguration config) {
        _oaiEndpoint = config["AzureOAIEndpoint"] ?? "";
        _oaiKey = config["AzureOAIKey"] ?? "";
        _oaiModelName = config["AzureOAIModelName"] ?? "";
        _azureSearchEndpoint = config["AzureSearchEndpoint"] ?? "";
        _azureSearchKey = config["AzureSearchKey"] ?? "";
        _azureSearchIndex = config["AzureSearchIndex"] ?? "";
    }
    public string GetEndpointConfig()
    {
        return _oaiEndpoint;
    }
}

