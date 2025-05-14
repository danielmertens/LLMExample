# LLM Examples

## Projects

- **LLMExample.Console.OpenAI**: Simplest example of how to connect with an LLM and create a conversation.
- **LLMExample.Console.OpenAIStreaming**: Example of how to stream results for the LLM for faster user feedback.
- **LLMExample.Console.ExtensionsAI**: Introduction of the Microsoft.Extensions.Ai package that introduces some handy tools and abstractions for using an LLM.
- **LLMExample.Console.StructuredData**: An example of how to make the LLM return structured data.
- **LLMExample.Console.Tools**: Simple example of adding functions to the abilities of the LLM.
- **LLMExample.Console.SK**: Example of using Semantic Kernel to add advanced functionality and plugins to the capabilities of the LLM.

## How to run

To run the project you will need to create a azure AI Foundry project with a model. Make sure the selected model is able to do chat completion and function calling.

Once the model is created you will need 3 pieces of information: The Endpoint, ApiKey and model name of your deployed model.

Add the Endpoint and model name to the OpenAI section in the appsetting of the project you want to run:

```
"OpenAI": {
    "Endpoint": "your-azure-project-endpoint.com",
    "DeploymentId": "your-model-name"
}
```

Add the apikey to the user secrets of the project by runnin the command:

```
dotnet user-secrets set "OpenAI:ApiKey" "Your-Api-Key"
```

Now you can run the project with the command `dotnet run`
