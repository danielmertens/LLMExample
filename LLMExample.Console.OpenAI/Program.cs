using Azure.AI.OpenAI;
using System.ClientModel;
using OpenAI.Chat;
using LLMExample.Shared;

// Build configuration to read appsettings.json
var openAiConfig = ConfigHelper.GetOpenAiConfig();

// Create Azure OpenAI client. This will be able to connect to your Foundry project.
var client = new AzureOpenAIClient(
    new Uri(openAiConfig.Endpoint),
    new ApiKeyCredential(openAiConfig.ApiKey));

// Create a chat client. This will be able to connect to the specific model you deployed.
var chatClient = client.GetChatClient(openAiConfig.DeploymentId);

// We will keep track of the conversation with the assistant.
var chatHistory = new List<ChatMessage>();

// Simple loop to talk to the LLM
while (true)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("User: ");
    var input = Console.ReadLine();

    // Add the user message to the conversation history
    chatHistory.Add(new UserChatMessage(input));

    // Send the full conversation to the model
    var chatCompletion = (await chatClient.CompleteChatAsync(chatHistory)).Value;

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{chatCompletion.Role}: {chatCompletion.Content[0].Text}");

    // Add the assistant message to the conversation history
    chatHistory.Add(new AssistantChatMessage(chatCompletion.Content[0].Text));
}
