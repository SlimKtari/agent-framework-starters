using System.ComponentModel;

namespace AgentFramework.MultiAgentsWorkflows.Plugins;

public class Budget
{
    [Description("Get current budget available.")]
    public string CurrentBudgetAvailable()
    {
        return "Current budget available is €10.000.";
    }
}