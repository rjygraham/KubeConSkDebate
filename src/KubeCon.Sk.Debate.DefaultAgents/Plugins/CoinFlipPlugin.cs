using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace KubeCon.Sk.Debate.DefaultAgents.Plugins;

public class CoinFlipPlugin
{
    [KernelFunction("flip_coin")]
    [Description("Flips a coin to determine heads or tails")]
    public async Task<string> FlipCoinAsync()
    {
        var result = Random.Shared.Next(0, 100) % 2 == 0 ? "heads" : "tails";
        return result;
    }
}
