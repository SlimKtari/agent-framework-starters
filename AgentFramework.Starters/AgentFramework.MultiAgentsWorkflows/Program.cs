using AgentFramework.MultiAgentsWorkflows.Extensions;
using static AgentFramework.MultiAgentsWorkflows.Extensions.AgentsExtensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.AddAppOpenApi();
builder.Services.AddControllers();

// Add AI platform services and agents
builder.AddAiPlatform();

// Initialize Azure Chat Client and register it for agent use.
builder.AddChatClient();

// Register event planning agents and get handles for mapping endpoints.
AgentHandles agents = builder.AddEventPlanningAgents();

var app = builder.Build();
app.MapControllers();
app.UseAppPipeline();
app.MapAppOpenApi();

// Map AI platform and agent endpoints.
app.MapAiPlatform();

// Map agent-specific endpoints for direct interactions if needed.
app.MapOpenAIChatCompletions(agents.LocationFinder);
app.MapOpenAIChatCompletions(agents.BudgetEstimator);
app.MapOpenAIChatCompletions(agents.LogisticsPlanner);
app.MapOpenAIChatCompletions(agents.SequentialWorkflow);
app.MapOpenAIChatCompletions(agents.ConcurrentWorkflow);
app.MapOpenAIChatCompletions(agents.GroupChatWorkflow);

app.Run();