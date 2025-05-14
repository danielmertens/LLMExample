using System.ClientModel;
using System.ComponentModel;
using Azure.AI.OpenAI;
using LLMExample.Shared;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var hostBuilder = Host.CreateApplicationBuilder(args);
var openAiConfig = ConfigHelper.GetOpenAiConfig();

var setupChatClient = new AzureOpenAIClient(
        new Uri(openAiConfig.Endpoint),
        new ApiKeyCredential(openAiConfig.ApiKey))
    .GetChatClient(openAiConfig.DeploymentId)
    .AsIChatClient();

// setup the client with the ability to call functions
hostBuilder.Services.AddChatClient(setupChatClient)
    .UseFunctionInvocation();

// setup logging
hostBuilder.Services.AddLogging(builder => builder
    .AddConsole()
    .SetMinimumLevel(LogLevel.Information));

var app = hostBuilder.Build();
var chatClient = app.Services.GetRequiredService<IChatClient>();

List<ChatMessage> chatHistory = 
[
    new(ChatRole.System, "You are a helpful assistant that is excited to sell the user a pizza.")
];

var cart = new Cart();
var getPriceTool = AIFunctionFactory.Create(cart.GetPrice);
var addPizzaTool = AIFunctionFactory.Create(cart.AddPizzaToCart);
var availablePizzaTool = AIFunctionFactory.Create(cart.GetAvailablePizzas);
var chatOptions = new ChatOptions
{
    Tools = [ getPriceTool, addPizzaTool, availablePizzaTool ],
    AllowMultipleToolCalls = true,
    ToolMode = ChatToolMode.Auto,
};

while (true)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("You: ");
    var input = Console.ReadLine();
    
    chatHistory.Add(new(ChatRole.User, input));

    var response = await chatClient.GetResponseAsync(chatHistory, chatOptions);

    foreach (var message in response.Messages)
    {
        if (message.Role == ChatRole.Assistant)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{message.Role.Value}: {message.Text}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{message.Role.Value}: {message.Contents[0]}");
        }
    }

    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response.Text));
}

public class Cart
{
    public List<Pizza> Pizzas { get; set; } = new();
    public float TotalPrice => Pizzas.Sum(p => p.Price);

    public void AddPizzaToCart(string type, int amount)
    {
        var pizza = new Pizza
        {
            Type = type,
            Amount = amount,
            Price = GetPrice(type, amount)
        };
        Pizzas.Add(pizza);

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"Added {amount} {type} pizza(s) to the cart. Total price: {pizza.Price}");
    }

    public string[] GetAvailablePizzas()
    {
        return new[] { "Margherita", "Pepperoni", "Hawaiian" };
    }

    [Description("Calculates the price of the pizza's. This returns the price of all pizza's as a total or -1 if the type of pizza is not available")]
    public float GetPrice(
    [Description("The type of pizza the user would like to order")] string pizzaType,
    [Description("The amount of pizza's the user would like to order")] int amount)
    {
        return pizzaType.ToLowerInvariant() switch
        {
            "margherita" => 8.99f,
            "pepperoni" => 9.99f,
            "hawaiian" => 10.99f,
            _ => -1
        } * amount;
    }
}

public class Pizza
{
    public string Type { get; set; } = string.Empty;
    public int Amount { get; set; }
    public float Price { get; set; }
}
