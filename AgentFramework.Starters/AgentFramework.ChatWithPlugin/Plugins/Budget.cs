using System.ComponentModel;
using Microsoft.Extensions.AI;

namespace AgentFramework.ChatWithPlugin.Plugins;

public class Budget : AITool
{
    [Description("Get current budget available.")]
    public string CurrentBudgetAvailable()
    {
        return "Current budget available is €10.000.";
    }
}