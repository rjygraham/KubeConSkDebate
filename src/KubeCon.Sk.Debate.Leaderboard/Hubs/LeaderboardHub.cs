using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Models;
using Microsoft.AspNetCore.SignalR;

namespace KubeCon.Sk.Debate.Leaderboard.Hubs;

public class LeaderboardHub : Hub<ILeaderboardGrainObserver>
{

    public async Task OnDebateTopicSet(string topic)
    {
        await Clients.All.OnDebateTopicSelected(topic);
    }

    public async Task OnDebateAgentsSelected(AgentDescriptor moderator, AgentDescriptor debater1, AgentDescriptor debater2)
    {
        await Clients.All.OnDebateAgentsSelected(moderator, debater1, debater2);
    }

    public async Task OnDebateStarted(DateTime startTime)
    {
        await Clients.All.OnDebateStarted(startTime);
    }

    public async Task OnDebateCompleted(DateTime endTime)
    {
        await Clients.All.OnDebateEnded(endTime);
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

    public async Task OnLobbyUpdated(List<Agent> agentsInLobby)
    {
        await Clients.All.OnLobbyUpdated(agentsInLobby);
    }

    public async Task OnSystemStatusUpdated(SystemStatusUpdate update)
    {
        await Clients.All.OnSystemStatusUpdated(update);
    }
}
