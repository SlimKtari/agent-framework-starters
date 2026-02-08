using Microsoft.Agents.AI.DevUI;

namespace AgentFramework.ChatWithPlugin.Extensions;
public static class AiPlatformExtensions
{
    public static WebApplicationBuilder AddAiPlatform(this WebApplicationBuilder builder)
    {
        builder.AddOpenAIResponses();
        builder.AddOpenAIChatCompletions();
        builder.Services.AddOpenAIConversations();
        builder.AddDevUI();
        return builder;
    }

    public static WebApplication MapAiPlatform(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapDevUI();
        }
        app.MapOpenAIResponses();
        app.MapOpenAIConversations();
        return app;
    }
}