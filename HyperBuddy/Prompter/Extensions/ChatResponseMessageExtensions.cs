using Azure.AI.OpenAI;
using System.Text.Json;

namespace HyperBuddy.Prompter.Extensions;
public static class ChatResponseMessageExtensions
{
    public static string? ContentPretty(this ChatResponseMessage contextMessage)
    {
        string? contextContent = null;
        try
        {
            var contextMessageJson = JsonDocument.Parse(contextMessage.Content);
            contextContent = JsonSerializer.Serialize(contextMessageJson, new JsonSerializerOptions()
            {
                WriteIndented = true,
            });
        }
        catch (JsonException)
        { }
        return contextContent;
    }
}

