using Microsoft.Agents.AI.Hosting;

namespace AgentFramework.HelloAgent.Extensions;

public static class AgentsExtensions
{
    private const string ChatClientKey = "chat-client";
    public sealed record AgentHandles(
        IHostedAgentBuilder LocationAgent,
        IHostedAgentBuilder BudgetAgent
    );

    public static AgentHandles AddEventPlanningAgents(this WebApplicationBuilder builder)
    {
        var location = builder.AddAIAgent(
                "location_finder",
                instructions: AgentPrompts.LocationFinder,
                description: "Suggests venues appropriate for an event.",
                chatClientServiceKey: ChatClientKey)
            .WithInMemorySessionStore();

        var budget = builder.AddAIAgent(
                "budget_estimator",
                instructions: AgentPrompts.BudgetEstimator,
                description: "Estimates costs and evaluates budget feasibility.",
                chatClientServiceKey: ChatClientKey)
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