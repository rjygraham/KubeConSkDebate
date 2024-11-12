using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace KubeCon.Sk.Debate.Grains;

public class MatchmakingGrain(ILogger<MatchmakingGrain> logger) : Grain, IMatchmakingGrain
{
    public async Task<(Agent, Agent, Agent)?> ChooseAgents()
    {
        var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
        var lobby = await lobbyGrain.GetAgentsInLobby();

        var moderators = lobby.Where(agent => agent.AgentType == AgentType.Moderator).ToArray();
        var debaters = lobby.Where(agent => agent.AgentType == AgentType.Debater).ToArray();

        logger.LogInformation("SkD: There are {Count} moderator agents in the lobby.", moderators.Length);
        logger.LogInformation("SkD: There are {Count} debater agents in the lobby.", debaters.Length);

        if (moderators is not { Length: >= 1 } || debaters is not { Length: >= 2 })
        {
            return default;
        }

        var moderator = Random.Shared.GetItems(moderators, 1).First();

        var pickDebaters = new Func<Agent[]>(() => Random.Shared.GetItems(debaters.ToArray(), 2));

        var selectedDebaters = pickDebaters();
        while (selectedDebaters[0].Name == selectedDebaters[1].Name)
        {
            selectedDebaters = pickDebaters();
        }

        await lobbyGrain.EnterDebate(moderator);
        await lobbyGrain.EnterDebate(selectedDebaters[0]);
        await lobbyGrain.EnterDebate(selectedDebaters[1]);

        return (moderator, selectedDebaters[0], selectedDebaters[1]);
    }
}