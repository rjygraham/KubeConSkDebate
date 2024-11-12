using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace KubeCon.Sk.Debate.Grains;
public class LobbyGrain : Grain, ILobbyGrain
{
    private readonly IPersistentState<List<Agent>> agentsInLobby;
    private readonly IPersistentState<List<Agent>> agentsSignedIn;
    private readonly ILogger<LobbyGrain> logger;
    private readonly ILeaderboardGrain leaderboardGrain;

    public LobbyGrain(
        [PersistentState("InLobby", storageName: "Lobby")] IPersistentState<List<Agent>> playersInLobby,
        [PersistentState("SignedIn", storageName: "Lobby")] IPersistentState<List<Agent>> playersSignedIn,
        ILogger<LobbyGrain> logger
    )
    {
        agentsInLobby = playersInLobby;
        agentsSignedIn = playersSignedIn;
        this.logger = logger;
        leaderboardGrain = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
    }

    public ValueTask<List<Agent>> GetAgentsInLobby() => ValueTask.FromResult(agentsInLobby.State);

    public async Task SignIn(Agent agent)
    {
        if (!agentsSignedIn.State.Any(x => x.Name == agent.Name))
        {
            logger.LogInformation("SkD: {AgentName} has entered the debate.", agent.Name);
            agentsSignedIn.State.Add(agent);
            await leaderboardGrain.AgentsOnlineUpdated(agentsSignedIn.State);
        }
    }

    public async Task EnterLobby(Agent agent)
    {
        if (!agentsInLobby.State.Any(x => x.Name == agent.Name))
        {
            logger.LogInformation("SkD: {AgentName} has entered the lobby.", agent.Name);
            agentsInLobby.State.Add(agent);
            await leaderboardGrain.LobbyUpdated(agentsInLobby.State);

            UpdateAgentLeaderboardRecord(agent);
            await leaderboardGrain.AgentsOnlineUpdated(agentsSignedIn.State);
        }
    }

    public async Task EnterDebate(Agent agent)
    {
        if (agentsInLobby.State.Any(x => x.Name == agent.Name))
        {
            logger.LogInformation("SkD: {AgentName} has left the lobby and entered the debate.", agent.Name);
            agentsInLobby.State.RemoveAll(x => x.Name == agent.Name);
            await leaderboardGrain.LobbyUpdated(agentsInLobby.State);
            await leaderboardGrain.AgentsOnlineUpdated(agentsSignedIn.State);
        }
    }

    public async Task SignOut(Agent agent)
    {
        if (agentsInLobby.State.Any(x => x.Name == agent.Name))
        {
            logger.LogInformation("SkD: {AgentName} has left the lobby.", agent.Name);
            agentsInLobby.State.RemoveAll(x => x.Name == agent.Name);
            await leaderboardGrain.LobbyUpdated(agentsInLobby.State);
        }

        if (agentsSignedIn.State.Any(x => x.Name == agent.Name))
        {
            logger.LogInformation("SkD: {AgentName} has left the game.", agent.Name);
            agentsSignedIn.State.RemoveAll(x => x.Name == agent.Name);
        }

        await leaderboardGrain.AgentsOnlineUpdated(agentsSignedIn.State);
    }

    private void UpdateAgentLeaderboardRecord(Agent agent)
    {
        if (agentsSignedIn.State.Any(x => x.Name == agent.Name))
        {
            agentsSignedIn.State.RemoveAll(x => x.Name == agent.Name);
            agentsSignedIn.State.Add(agent);
        }
    }
}
