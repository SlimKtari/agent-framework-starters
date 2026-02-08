using Scalar.AspNetCore;

namespace AgentFramework.MultiAgentsWorkflows.Extensions;
public static class OpenApiExtensions
{
    public static WebApplicationBuilder AddAppOpenApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        return builder;
    }

    public static WebApplication MapAppOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference("docs");
        }
        return app;
    }
}