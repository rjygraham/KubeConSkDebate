using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Models;
using Microsoft.AspNetCore.SignalR;

namespace KubeCon.Sk.Debate.Leaderboard.Hubs;

public class LeaderboardHub : Hub<ILeaderboardGrainObserver>
{
    public async Task OnDebateStarted(Abstractions.Models.Debate debate)
    {
        await Clients.All.OnDebateStarted(debate);
    }

    public async Task OnAgentScoresUpdated(Agent agent)
    {
        await Clients.All.OnAgentScoresUpdated(agent);
    }

    public async Task OnDebateChatMessageAdded(ChatMessage message)
    {
        await Clients.All.OnDebateChatMessageAdded(message);
    }

    public async Task OnAgentsOnlineUpdated(List<Agent> agentsOnline)
    {
        await Clients.All.OnAgentsOnlineUpdated(agentsOnline);
    }

    public async Task OnDebateCompleted(Abstractions.Models.Debate debate)
    {
        await Clients.All.OnDebateCompleted(debate);
    }

    public async Task OnLobbyUpdated(List<Agent> agentsInLobby)
    {
        await Clients.All.OnLobbyUpdated(agentsInLobby);
    }

    public async Task OnSystemStatusUpdated(SystemStatusUpdate update)
    {
        await Clients.All.OnSystemStatusUpdated(update);
    }
}
