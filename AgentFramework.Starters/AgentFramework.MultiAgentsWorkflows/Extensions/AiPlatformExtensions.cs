using Microsoft.Agents.AI.DevUI;

namespace AgentFramework.MultiAgentsWorkflows.Extensions;
public static class AiPlatformExtensions
{
    public static WebApplicationBuilder AddAiPlatform(this WebApplicationBuilder builder)
    {
        // Register services for OpenAI responses and conversations (required for DevUI)
        builder.Services.AddOpenAIResponses();
        builder.Services.AddOpenAIConversations();
        builder.Services.AddOpenAIChatCompletions();
        builder.Services.AddDevUI();
        return builder;
    }

    public static WebApplication MapAiPlatform(this WebApplication app)
    {
        // Map endpoints for OpenAI responses and conversations (required for DevUI).
        app.MapOpenAIResponses();
        app.MapOpenAIConversations();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapDevUI();
        }

        return app;
    }
}