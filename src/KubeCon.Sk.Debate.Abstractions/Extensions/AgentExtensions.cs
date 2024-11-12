using KubeCon.Sk.Debate.Abstractions.Models;

namespace KubeCon.Sk.Debate.Abstractions.Extensions;

public static class AgentExtensions
{
    public static double CalculateWinPercentage(this Agent agent)
    {
        try
        {
            double pct = agent.WinCount / (double)agent.TotalDebates;
            return Math.Round(pct * 100, 0);
        }
        catch
        {
            return 0;
        }
    }
}
