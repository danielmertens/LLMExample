using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace LLMExample.Web.Services
{
    public interface IChatService
    {
        Task<ChatMessage> SendMessageAsync(List<ChatMessage> history);
    }

    public class ChatService : IChatService
    {
        private readonly AzureOpenAIClient _aiClient;
        private readonly ChatClient _chatClient;

        public ChatService(IConfiguration configuration)
        {
            var endpoint = configuration["OpenAI:Endpoint"];
            var apiKey = configuration["OpenAI:ApiKey"];
            var deploymentId = configuration["OpenAI:DeploymentId"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(deploymentId))
            {
                throw new ArgumentException("OpenAI configuration is missing or invalid.");
            }

            _aiClient = new(
                new Uri(endpoint),
                new ApiKeyCredential(apiKey));
            _chatClient = _aiClient.GetChatClient(deploymentId);
        }

        public async Task<ChatMessage> SendMessageAsync(List<ChatMessage> history)
        {
            var response = await _chatClient.CompleteChatAsync([
                new SystemChatMessage(""),
                ..history,
                ]);

            if (response == null)
            {
                return new AssistantChatMessage("Failed to get a response");
            }

            return new AssistantChatMessage(response.Value);
        }
    }
}
