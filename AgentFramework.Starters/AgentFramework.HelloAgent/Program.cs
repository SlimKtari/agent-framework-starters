using AgentFramework.HelloAgent.Extensions;
using static AgentFramework.HelloAgent.Extensions.AgentsExtensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.AddAppOpenApi();
builder.Services.AddControllers();

// Initialize Azure Chat Client and register it for agent use.
builder.AddChatClient();

// Register event planning agents and get handles for mapping endpoints.
AgentHandles agents = builder.AddEventPlanningAgents();

// Add AI platform services and agents
builder.AddAiPlatform();

var app = builder.Build();
app.MapControllers();
app.UseAppPipeline();
app.MapAppOpenApi();

// Map AI platform and agent endpoints.
app.MapAiPlatform();

// Map agent-specific endpoints for direct interactions if needed.
app.MapOpenAIChatCompletions(agents.LocationAgent);
app.MapOpenAIChatCompletions(agents.BudgetAgent);

app.Run();