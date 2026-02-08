using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AgentFramework.MultiAgentsWorkflows.WorkflowTools;

/// <summary>
/// Custom GroupChatManager that selects the next speaker based on the conversation flow.
/// </summary>
internal sealed class CustomGroupChatManager(IReadOnlyList<AIAgent> agents) : GroupChatManager
{
    protected override ValueTask<AIAgent> SelectNextAgentAsync(
        IReadOnlyList<ChatMessage> history,
        CancellationToken cancellationToken = default)
    {
        if (history.Count == 0)
        {
            throw new InvalidOperationException("Conversation is empty; cannot select next speaker.");
        }

        // First speaker after initial user message
        if (this.IterationCount == 0)
        {
            AIAgent locationFinder = agents.First(a => a.Name == "location_finder");
            return new ValueTask<AIAgent>(locationFinder);
        }

        if (this.IterationCount == 1)
        {
            AIAgent budgetEstimator = agents.First(a => a.Name == "budget_estimator");
            return new ValueTask<AIAgent>(budgetEstimator);
        }

        AIAgent logisticsPlanner = agents.First(a => a.Name == "logistics_planner");
        return new ValueTask<AIAgent>(logisticsPlanner);
    }

    protected override ValueTask<IEnumerable<ChatMessage>> UpdateHistoryAsync(IReadOnlyList<ChatMessage> history, CancellationToken cancellationToken = new CancellationToken())
    {
        return base.UpdateHistoryAsync(history, cancellationToken);
    }

    protected override ValueTask<bool> ShouldTerminateAsync(IReadOnlyList<ChatMessage> history, CancellationToken cancellationToken = new CancellationToken())
    {
        return base.ShouldTerminateAsync(history, cancellationToken);
    }
}