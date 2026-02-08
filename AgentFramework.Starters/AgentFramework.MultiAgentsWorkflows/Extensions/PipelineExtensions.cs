namespace AgentFramework.MultiAgentsWorkflows.Extensions;
public static class PipelineExtensions
{
    public static WebApplication UseAppPipeline(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseExceptionHandler();
        return app;
    }
}