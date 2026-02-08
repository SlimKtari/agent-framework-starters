using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;

namespace AgentFramework.MultiAgentsWorkflows.Extensions;

public static class ChatClientExtensions
{
    private const string ChatClientKey = "chat-client";

    public static WebApplicationBuilder AddChatClient(this WebApplicationBuilder builder)
    {
        var endpoint = builder.Configuration["Azure:Foundry:ChatCompletion:Endpoint"]
            ?? throw new InvalidOperationException("Endpoint for Azure Foundry Chat Completion model is not set.");

        var deploymentName = builder.Configuration["Azure:Foundry:ChatCompletion:DeploymentName"]
            ?? throw new InvalidOperationException("DeploymentName for Azure Foundry Chat Completion model is not set.");

        var apiKey = builder.Configuration["Azure:Foundry:ChatCompletion:ApiKey"]
            ?? throw new InvalidOperationException("ApiKey for Azure Foundry Chat Completion model is not set.");

        var chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .AsIChatClient();

        builder.Services.AddKeyedSingleton<IChatClient>(ChatClientKey, chatClient);
        //builder.Services.AddChatClient(chatClient, ServiceLifetime.Singleton);

        return builder;
    }
}