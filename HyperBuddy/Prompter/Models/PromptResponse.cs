using Azure.AI.OpenAI;
using HyperBuddy.Prompter.Constants;

namespace HyperBuddy.Prompter.Models
{
    public class PromptResponse
    {
        private ChatRole _role;
        public string? Role {
            get {
                return _role.ToString();
            } 
            set { 
                _role = new ChatRole(value);
            } 
        }

        public string? MsgContent { get; set; }
        public List<ChatResponseDataContext?> DataContext { get; set; }
        public PromptResponse() {
            DataContext = new();
        }
    }
    
}
