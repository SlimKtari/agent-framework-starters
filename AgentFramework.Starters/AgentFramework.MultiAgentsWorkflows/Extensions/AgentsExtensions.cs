using AgentFramework.MultiAgentsWorkflows.Models;
using AgentFramework.MultiAgentsWorkflows.Plugins;
using AgentFramework.MultiAgentsWorkflows.WorkflowTools;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AgentFramework.MultiAgentsWorkflows.Extensions;

public static class AgentsExtensions
{
    private const string ChatClientKey = "chat-client";

    public sealed record AgentHandles(
        IHostedAgentBuilder BudgetEstimator,
        IHostedAgentBuilder LocationFinder,
        IHostedAgentBuilder LogisticsPlanner,
        IHostedAgentBuilder SequentialWorkflow,
        IHostedAgentBuilder ConcurrentWorkflow,
        IHostedAgentBuilder GroupChatWorkflow);
    
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

        var logistics = builder.AddAIAgent(
                "logistics_planner",
                instructions: AgentPrompts.LogisticsPlanner,
                description: "Plans and evaluates event logistics requirements.",
                chatClientServiceKey: ChatClientKey)
            .WithInMemorySessionStore();

        IHostedAgentBuilder sequentialWorkflow = builder.AddWorkflow("sequential-workflow", (sp, key) =>
        {
            var agents = new List<IHostedAgentBuilder> { location, budget, logistics }
                .Select(ab => sp.GetRequiredKeyedService<AIAgent>(ab.Name));
            return AgentWorkflowBuilder.BuildSequential(workflowName: key, agents: agents);
        }).AddAsAIAgent();

        IHostedAgentBuilder concurrentWorkflow = builder.AddWorkflow("concurrent-workflow", (sp, key) =>
        {
            var agents = new List<IHostedAgentBuilder> { location, budget, logistics }
                .Select(ab => sp.GetRequiredKeyedService<AIAgent>(ab.Name));
            return AgentWorkflowBuilder.BuildConcurrent(workflowName: key, agents: agents);
        }).AddAsAIAgent();
        
        IHostedAgentBuilder groupChatWorkflow = builder.AddAIAgent("groupChat-workflow", (sp, key) =>
        {
            var agents = new List<IHostedAgentBuilder> { location, budget, logistics }
                .Select(ab => sp.GetRequiredKeyedService<AIAgent>(ab.Name))
                .ToList();
            var manager = new CustomGroupChatManager(agents) { MaximumIterationCount = 5 };
            var workflow = AgentWorkflowBuilder.CreateGroupChatBuilderWith(_ => manager)
                .AddParticipants(agents)
                .Build();
            return workflow.AsAgent(name: key, description:"Multi-agent group chat workflow.");
        });

        return new AgentHandles(
            budget,
            location,
            logistics,
            sequentialWorkflow, 
            concurrentWorkflow, 
            groupChatWorkflow);
    }

    private static class AgentPrompts
    {
        public const string Coordinator = """
            You are the Coordinator Agent responsible for orchestrating event planning.
            Your role is to understand the user’s request, delegate tasks to other agents,
            and combine their responses into a final recommendation.

            Always:
            - Ask clarifying questions if information is missing
            - Call other agents when needed
            - Summarize results clearly
            - Prepare output for human approval when required
            """;

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

        public const string LogisticsPlanner = """
            You are the Logistics Planner Agent.

            Your responsibility is to plan and coordinate the operational aspects of an event.
            This includes evaluating requirements such as:

            - equipment (projectors, audio, seating)
            - transportation
            - setup needs
            - technical resources
            - accessibility considerations

            You should provide structured recommendations including:
            - required logistics items
            - potential challenges
            - optimization suggestions

            Do not make final decisions.
            Provide clear and actionable planning input for the Coordinator Agent.
            """;
    }
}