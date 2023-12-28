using Azure;
using Azure.AI.OpenAI;
using System.Text.Json;
using HyperBuddy.Prompter.Models;

namespace HyperBuddy.Prompter.Services;

public interface IPromptingService
{
    PromptResponse PromptCogSearch(List<ChatMessage> chatHistory);
}
public class OpenAIPromptingService : IPromptingService
{
    private readonly string _oaiEndpoint;
    private readonly string _oaiKey;
    private readonly string _oaiModelName;
    private readonly string _azureSearchEndpoint;
    private readonly string _azureSearchKey;
    private readonly string _azureSearchIndex;

    private readonly ChatRequestSystemMessage _defaultSysMsg = new("You are an AI assistant that helps navigate a chef through a recipe");

    private readonly OpenAIClient _openAIClient;
    public OpenAIPromptingService(IConfiguration config)
    {
        _oaiEndpoint = config["OaiConfigs:AzureOAIEndpoint"] ?? "";
        _oaiKey = config["OaiConfigs:AzureOAIKey"] ?? "";
        _oaiModelName = config["OaiConfigs:AzureOAIModelName"] ?? "";
        _azureSearchEndpoint = config["OaiConfigs:AzureSearchEndpoint"] ?? "";
        _azureSearchKey = config["OaiConfigs:AzureSearchKey"] ?? "";
        _azureSearchIndex = config["OaiConfigs:AzureSearchIndex"] ?? "";

        _openAIClient = new OpenAIClient(new Uri(_oaiEndpoint), new AzureKeyCredential(_oaiKey));
    }
    public PromptResponse PromptCogSearch(List<ChatMessage> chatHistory)
    {
        // configures the exteions for cognitive search (mainly the endpoint and keys)
        var ownDataConfig = CreateCogSearchExtensionConfig();

        // configures the chat completion to send to the client, including user message
        var chatCompletionOptions = CreateChatCompletionOptions(chatHistory, ownDataConfig);

        // send data via client to open AI
        ChatCompletions response = _openAIClient.GetChatCompletions(chatCompletionOptions);
        var responseMessage = response.Choices[0].Message;


        // handle the response and build the response objs
        PromptResponse chatHistoryResponse = new()
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
            catch (Exception) { }
        }
        return chatHistoryResponse;
    }

    #region Private Methods
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

    /// <summary>
    /// Method to create the new Chat Completion with Az Search Extension.
    /// </summary>
    /// <param name="userMessage"></param>
    /// <param name="ownDataConfig"></param>
    /// <returns></returns>
    private ChatCompletionsOptions CreateChatCompletionOptions(List<ChatMessage> chatHistory, AzureCognitiveSearchChatExtensionConfiguration ownDataConfig)
    {
        // create the base object
        ChatCompletionsOptions chatCompletionsOptions = new()
        {
            MaxTokens = 600,
            Temperature = 0.9f,
            DeploymentName = _oaiModelName,
            // Specify extension options to use Azure Search
            AzureExtensionsOptions = new AzureChatExtensionsOptions()
            {
                Extensions = { ownDataConfig }
            },  
        };

        // add the system message: "You are an AI assistant that helps with recipe..."
        chatCompletionsOptions.Messages.Add(_defaultSysMsg);

        // add the chat history
        chatHistory.ForEach(msg => 
            chatCompletionsOptions.Messages.Add(msg.ToChatRequestMessage()));

        return chatCompletionsOptions;
    }
    #endregion
}

