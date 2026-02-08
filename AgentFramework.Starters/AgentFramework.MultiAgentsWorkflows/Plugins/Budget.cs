using System.ComponentModel;
using Microsoft.Extensions.AI;

namespace AgentFramework.AgentWithStructuredOutput.Plugins;

public class Budget : AITool
{
    [Description("Get current budget available.")]
    public string CurrentBudgetAvailable()
    {
        return "Current budget available is €10.000.";
    }
}