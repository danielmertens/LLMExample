using System.ClientModel;
using Azure.AI.OpenAI;
using LLMExample.Shared;
using Microsoft.Extensions.AI;

var openAiConfig = ConfigHelper.GetOpenAiConfig();

var client = new AzureOpenAIClient(
    new Uri(openAiConfig.Endpoint),
    new ApiKeyCredential(openAiConfig.ApiKey));

// Here we use the Microsoft.Extensions.AI library
IChatClient chatClient = client.GetChatClient(openAiConfig.DeploymentId)
    .AsIChatClient();

var chatHistory = new List<ChatMessage>();

while (true)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("User: ");
    var input = Console.ReadLine();
    chatHistory.Add(new(ChatRole.User, input));

    var response = await chatClient.GetResponseAsync(chatHistory);

    Console.ForegroundColor = ConsoleColor.Green;
    foreach (var message in response.Messages)
    {
        Console.WriteLine($"{message.Role.Value}: {message.Text}");
    }

    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response.Text));
}



