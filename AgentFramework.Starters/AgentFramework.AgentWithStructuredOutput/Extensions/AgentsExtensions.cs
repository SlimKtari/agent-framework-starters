using AgentFramework.AgentWithStructuredOutput.Models;
using AgentFramework.AgentWithStructuredOutput.Plugins;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;

namespace AgentFramework.AgentWithStructuredOutput.Extensions;

public static class AgentsExtensions
{
    private const string ChatClientKey = "chat-client";
    public sealed record AgentHandles(
        IHostedAgentBuilder LocationAgent,
        IHostedAgentBuilder BudgetAgent);

    public static AgentHandles AddEventPlanningAgents(this WebApplicationBuilder builder)
    {
        var location = builder.AddAIAgent("location_finder", (sp, key) =>
        {
            var chatClient = sp.GetRequiredKeyedService<IChatClient>("chat-client");
            
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Name = key,
                Description = "Suggests venues appropriate for an event.",
                ChatOptions = new()
                {
                    Instructions = AgentPrompts.LocationFinder,
                    
                    MaxOutputTokens = 1024,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0,
                    Temperature = 0,
                    TopP = 0,
                    ToolMode = new AutoChatToolMode(),

                    ResponseFormat = ChatResponseFormat.ForJsonSchema<LocationFinderResult>()
                }
            });
        })
            .WithAITool(AIFunctionFactory.Create(new LocationsCatalog().LookupRooms))
            .WithInMemorySessionStore();

        var budget = builder.AddAIAgent(
                "budget_estimator",
                instructions: AgentPrompts.BudgetEstimator,
                description: "Estimates costs and evaluates budget feasibility.",
                chatClientServiceKey: ChatClientKey)
            .WithAITool(AIFunctionFactory.Create(new Budget().CurrentBudgetAvailable))
            .WithInMemorySessionStore();

        return new AgentHandles(location, budget);
    }
    
    private static class AgentPrompts
    {
        public const string LocationFinder = """
            You are the Location Finder Agent.

            Your role is to propose suitable event location based on requirements such as:
            - number of attendees
            - location
            - event type
            - budget constraints

            You may use mock or predefined data if real data is unavailable.
            Provide 2-3 options including:
            - name
            - estimated cost
            - short justification

            Do not make final decisions — only provide recommendations.
            """;

        public const string BudgetEstimator = """
            You are the Budget Estimator Agent.

            Your role is to estimate total event costs based on inputs such as:
            - venue cost
            - catering
            - equipment
            - number of attendees

            Return:
            - itemized estimate
            - total cost
            - warning if exceeding budget
            - possible cost-saving suggestions

            You do not approve decisions — only analyze financial feasibility.
            """;
    }
}