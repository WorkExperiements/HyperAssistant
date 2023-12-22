namespace HyperBuddy.Prompter.Models
{
    public class ChatResponseObjViewModel
    {
        public string? Role { get; set; }

        public string? MsgContent { get; set; }
        public List<ChatResponseDataContext?> DataContext { get; set; }
        public ChatResponseObjViewModel() {
            DataContext = new();
        }
    }
    
}
