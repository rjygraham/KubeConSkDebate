using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Extensions;
using KubeCon.Sk.Debate.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace KubeCon.Sk.Debate.Grains;

internal class AgentSessionGrain
(
    [PersistentState("Agents", storageName: "Agents")]
    IPersistentState<Agent> agent,
    ILogger<AgentSessionGrain> logger
) : Grain, IAgentSessionGrain
{
    private IAgentGrain? agentGrain;
   
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        agent.State.Name = this.GetPrimaryKeyString();

        await base.OnActivateAsync(cancellationToken);
    }

    public async Task<string> InvokeAsync(Guid debateId)
    {
        await agentGrain.InvokeAsync(debateId);

        return string.Empty;
    }

    public ValueTask<Agent> Get() => ValueTask.FromResult(agent.State);

    public ValueTask<bool> IsAgentOnline() => ValueTask.FromResult(agent.State.IsActive);

    public ValueTask<bool> IsAgentKicked() => ValueTask.FromResult(agent.State.IsKicked);

    public async Task SignIn(string agentFullName)
    {
        agentGrain = GrainFactory.GetGrain<IAgentGrain>(agent.State.Name, grainClassNamePrefix: agentFullName);
        logger.LogInformation("SkD: Agent {AgentName} has signed in.", agent.State.Name);
        agent.State.AgentType = await agentGrain.GetRole();
        agent.State.FullName = agentFullName;
        agent.State.Instructions = await agentGrain.GetInstructions();
        agent.State.IsActive = true;
        await agent.WriteStateAsync();

        var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
        await lobbyGrain.SignIn(agent.State);
        await lobbyGrain.EnterLobby(agent.State);
    }

    public async Task SignOut()
    {
        agent.State.IsActive = false;

        logger.LogInformation("SkD: Agent {AgentName} has signed out.", agent.State.Name);

        agentGrain = null;

        var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);
        await lobbyGrain.SignOut(agent.State);
        await agent.WriteStateAsync();
    }

    public async Task SetOpponent(string opponent)
    {
        agent.State.CurrentOpponent = opponent;
        await agent.WriteStateAsync();
    }

    public async Task RecordLoss()
    {
        logger.LogInformation("SkD: Recording loss for {AgentName}", agent.State.Name);
        agent.State.TotalDebates += 1;
        agent.State.LossCount += 1;
        agent.State.PercentWon = (int)agent.State.CalculateWinPercentage();
        await agent.WriteStateAsync();
        await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).AgentScoresUpdated(agent.State);
    }

    public async Task RecordWin()
    {
        logger.LogInformation("SkD: Recording win for {AgentName}", agent.State.Name);
        agent.State.TotalDebates += 1;
        agent.State.WinCount += 1;
        agent.State.PercentWon = (int)agent.State.CalculateWinPercentage();
        await agent.WriteStateAsync();
        await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).AgentScoresUpdated(agent.State);
    }

    public async Task RecordTie()
    {
        logger.LogInformation("SkD: Recording tie for {PlayerName}", agent.State.Name);
        agent.State.TotalDebates += 1;
        agent.State.TieCount += 1;
        agent.State.PercentWon = (int)agent.State.CalculateWinPercentage();
        await agent.WriteStateAsync();
        await GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty).AgentScoresUpdated(agent.State);
    }

    public async Task KickAgent()
    {
        agent.State.IsKicked = true;
        await SignOut();
    }

    public async Task UnkickAgent()
    {
        agent.State.IsKicked = false;
        await SignIn(agent.State.FullName!);
    }
}