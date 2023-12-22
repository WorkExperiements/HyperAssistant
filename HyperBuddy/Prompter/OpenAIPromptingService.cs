using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using System.Text.Json;
using HyperBuddy.Prompter.Extensions;
using static System.Net.Mime.MediaTypeNames;
using HyperBuddy.Prompter.Models;

namespace HyperBuddy.Prompter;

public interface IPromptingService
{
    ChatResponseObjViewModel PromptCogSearch(string userMsg);
}
public class OpenAIPromptingService : IPromptingService
{
    private readonly string _oaiEndpoint;
    private readonly string _oaiKey;
    private readonly string _oaiModelName;
    private readonly string _azureSearchEndpoint;
    private readonly string _azureSearchKey;
    private readonly string _azureSearchIndex;

    private readonly OpenAIClient _openAIClient;
    public OpenAIPromptingService(IConfiguration config) {
        _oaiEndpoint = config["OaiConfigs:AzureOAIEndpoint"] ?? "";
        _oaiKey = config["OaiConfigs:AzureOAIKey"] ?? "";
        _oaiModelName = config["OaiConfigs:AzureOAIModelName"] ?? "";
        _azureSearchEndpoint = config["OaiConfigs:AzureSearchEndpoint"] ?? "";
        _azureSearchKey = config["OaiConfigs:AzureSearchKey"] ?? "";
        _azureSearchIndex = config["OaiConfigs:AzureSearchIndex"] ?? "";

        _openAIClient = new OpenAIClient(new Uri(_oaiEndpoint), new AzureKeyCredential(_oaiKey));
    }
    public ChatResponseObjViewModel PromptCogSearch(string userMsg)
    {
        

        // configures the exteions for cognitive search (mainly the endpoint and keys)
        var ownDataConfig = CreateCogSearchExtensionConfig();
        
        // configures the chat completion to send to the client, including user message
        var chatCompletionOptions = CreateChatCompletionOptions(userMsg, ownDataConfig);

        // send data via client to open AI
        ChatCompletions response = _openAIClient.GetChatCompletions(chatCompletionOptions);
        var responseMessage = response.Choices[0].Message;


        // handle the response and build the response objs
        ChatResponseObjViewModel chatHistoryResponse = new()
        {
            MsgContent = responseMessage.Content ?? "*ERROR*",
            Role = responseMessage.Role.ToString()
        };

        
        // grab the data informed context and citations
        foreach (var contextMessage in responseMessage.AzureExtensionsContext.Messages.Where(cm => cm is not null))
        {
            try
            {
                var parsedObj = JsonSerializer.Deserialize<ChatResponseDataContext>(contextMessage.Content);
                if (parsedObj is not null)
                    chatHistoryResponse.DataContext.Add(parsedObj);
            }
            catch(Exception) { }
        }
        return chatHistoryResponse;
    }

    private AzureCognitiveSearchChatExtensionConfiguration CreateCogSearchExtensionConfig()
    {
        // Create extension config for own data
        AzureCognitiveSearchChatExtensionConfiguration ownDataConfig = new()
        {
            SearchEndpoint = new Uri(_azureSearchEndpoint),
            IndexName = _azureSearchIndex,
            Key = _azureSearchKey
        };
        return ownDataConfig;
    }

    private ChatCompletionsOptions CreateChatCompletionOptions(string userMessage, AzureCognitiveSearchChatExtensionConfiguration ownDataConfig)
    {
        ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Messages =
            {
                new ChatRequestUserMessage(userMessage)
            },
            MaxTokens = 600,
            Temperature = 0.9f,
            DeploymentName = _oaiModelName,
            // Specify extension options
            AzureExtensionsOptions = new AzureChatExtensionsOptions()
            {
                Extensions = { ownDataConfig }
            }
        };
        return chatCompletionsOptions;
    }
}

