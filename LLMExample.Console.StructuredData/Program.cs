using Azure.AI.OpenAI;
using LLMExample.Shared;
using Microsoft.Extensions.AI;
using Newtonsoft.Json;
using System.ClientModel;

var openAiConfig = ConfigHelper.GetOpenAiConfig();

var client = new AzureOpenAIClient(
    new Uri(openAiConfig.Endpoint),
    new ApiKeyCredential(openAiConfig.ApiKey));

IChatClient chatClient = client.GetChatClient(openAiConfig.DeploymentId).AsIChatClient();


var systemPrompt = $"""
    You are a helpfull assistant that is capable in incident management.
    When a incident comes in through our ticketing system, you are able to extract the important information from the ticket and create a structured incident.
    Make sure all information about the incident is returned in english.

    If there is no incident id mentioned in the ticket use id 456.

    For severity you can use Low, Medium or High. Use your best judgement to determine the severity of the incident.
    - Low: The incident has a impact on a single user at 1 customer and is not blocking them from working.
    - Medium: The incident has a impact on a group of users or on multiple customers.
    - High: The incident has a impact on all users or on all customers.

    The current date is {DateTime.Now:u}
    """;


// give an examle of a low impact incident
var exampleIncident1 = """
    mail from: ruben@tourism-madrid.es
    Hola, tengo un problema con myProtime.  
    Estoy intentando actualizar el horario de trabajo de uno de nuestros empleados, pero los cambios no se están guardando.  
    Esto parece estar ocurriendo solo con este empleado, ya que he intentado actualizar los horarios de otros y funciona bien.  
    El empleado en cuestión recientemente tuvo su rol actualizado en el sistema, así que no estoy seguro si eso podría estar relacionado con el problema.  
    No es urgente, ya que todavía puedo ver sus detalles y realizar otras tareas, pero agradecería que alguien pudiera revisar esto.  
    Por favor, avísenme si necesitan información adicional o acceso a la cuenta para investigar más a fondo.  
    ¡Gracias!  
    """;

var exampleIncident2 = """
    mail from: bert@protime.eu
    Hello, we are experiencing a critical issue with myProtime. 
    None of our users across all locations are able to log in to the system. 
    This is preventing everyone from accessing schedules, time tracking, and other essential features. 
    The issue seems to be affecting all customers, and it’s causing a major disruption to our operations.
    Thank you.
    """;

List<ChatMessage> chatHistory =
[
    new(ChatRole.System, systemPrompt),
    new(ChatRole.User, exampleIncident1)
];

var response = await chatClient.GetResponseAsync<IncidentDetails>(chatHistory);

if (response.TryGetResult(out var incident1))
{
    Console.WriteLine(JsonConvert.SerializeObject(incident1, Formatting.Indented));
}
else
{
    Console.WriteLine("No incident found");
}

chatHistory =
[
    new(ChatRole.System, systemPrompt),
    new(ChatRole.User, exampleIncident2)
];

response = await chatClient.GetResponseAsync<IncidentDetails>(chatHistory);

if (response.TryGetResult(out var incident2))
{
    Console.WriteLine(JsonConvert.SerializeObject(incident2, Formatting.Indented));
}
else
{
    Console.WriteLine("No incident found");
}

Console.ReadKey();

internal class IncidentDetails
{
    public string? IncidentId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Customer { get; set; }
    public string? Severity { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
}
