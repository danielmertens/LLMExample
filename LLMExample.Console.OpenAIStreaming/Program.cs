using Azure.AI.OpenAI;
using System.ClientModel;
using OpenAI.Chat;
using System.Text;
using LLMExample.Shared;

var openAiConfig = ConfigHelper.GetOpenAiConfig();

var client = new AzureOpenAIClient(
    new Uri(openAiConfig.Endpoint),
    new ApiKeyCredential(openAiConfig.ApiKey));
var chatClient = client.GetChatClient(openAiConfig.DeploymentId);

var chatHistory = new List<ChatMessage>();

while (true)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("User: ");
    var input = Console.ReadLine();
    chatHistory.Add(new UserChatMessage(input));

    // Stream the AI response
    var completionUpdates = chatClient.CompleteChatStreamingAsync(chatHistory);

    StringBuilder completeResponse = new();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("Assistant: ");

    // Wait for a streamed update, and immediatly print it to the console
    await foreach (var update in completionUpdates)
    {
        foreach (var contentPart in update.ContentUpdate)
        {
            Console.Write(contentPart.Text);
            completeResponse.Append(contentPart.Text);
        }
    }
    Console.WriteLine();

    chatHistory.Add(new AssistantChatMessage(completeResponse.ToString()));
}
