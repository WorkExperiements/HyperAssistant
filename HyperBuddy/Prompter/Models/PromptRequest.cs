using Azure.AI.OpenAI;
using System.Text.Json.Serialization;
using HyperBuddy.Prompter.Constants;

namespace HyperBuddy.Prompter.Models
{
    public class PromptRequest
    {
        [JsonPropertyName("history")]
        public List<ChatMessage>? History { get; set; }
    }
    public class ChatMessage
    {
        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("Content")]
        public string? Content { get; set; }

        public ChatRequestMessage ToChatRequestMessage()
        {
            if(From?.Equals(ChatActors.USER, StringComparison.CurrentCultureIgnoreCase) ?? false)
            {
                return new ChatRequestUserMessage(Content);                 
            }
            else
            {
                return new ChatRequestSystemMessage(Content);
            }
        }
    }
}
