using Microsoft.Extensions.Configuration;

namespace LLMExample.Shared;

public static class ConfigHelper
{
    public static OpenAiConfig GetOpenAiConfig()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddUserSecrets<OpenAiConfig>()
            .Build();

        var openAISection = config.GetSection("OpenAI");

        return new OpenAiConfig
        {
            Endpoint = openAISection["Endpoint"] ?? throw new ArgumentException("Endpoint is needed in config OpenAI:Endpoint"),
            ApiKey = openAISection["ApiKey"] ?? throw new ArgumentException("ApiKey is needed in config OpenAI:ApiKey"),
            DeploymentId = openAISection["DeploymentId"] ?? throw new ArgumentException("DeploymentId is needed in config OpenAI:DeploymentId")
        };
    }
}

public class OpenAiConfig
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DeploymentId { get; set; } = string.Empty;
}
