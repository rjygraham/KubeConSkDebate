using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Models;
using KubeCon.Sk.Debate.Leaderboard.Hubs;
using Markdig;
using Microsoft.AspNetCore.SignalR;

namespace KubeCon.Sk.Debate.Leaderboard;

public class LeaderboardObserver(IHubContext<LeaderboardHub, ILeaderboardGrainObserver> hub) : ILeaderboardGrainObserver
{
    public async Task OnDebateTopicSelected(string topic)
    {
        await hub.Clients.All.OnDebateTopicSelected(topic);
    }
    public async Task OnAgentScoresUpdated(Agent agent)
    {
        await hub.Clients.All.OnAgentScoresUpdated(agent);
    }

    public async Task OnAgentsOnlineUpdated(List<Agent> agentsOnline)
    {
        await hub.Clients.All.OnAgentsOnlineUpdated(agentsOnline);
    }

    public async Task OnDebateAgentsSelected(AgentDescriptor moderator, AgentDescriptor debater1, AgentDescriptor debater2)
    {
        await hub.Clients.All.OnDebateAgentsSelected(moderator, debater1, debater2);
    }

    public async Task OnDebateStarted(DateTime startTime)
    {
        await hub.Clients.All.OnDebateStarted(startTime);
    }

    public async Task OnDebateChatMessageAdded(ChatMessage message)
    {
        var html = Markdown.ToHtml(message.Content);
        message.Content = html;
        await hub.Clients.All.OnDebateChatMessageAdded(message);
    }

    public async Task OnDebateEnded(DateTime endTime)
    {
        await hub.Clients.All.OnDebateEnded(endTime);
    }

    public async Task OnLobbyUpdated(List<Agent> agentsInLobby)
    {
        await hub.Clients.All.OnLobbyUpdated(agentsInLobby);
    }

    public async Task OnSystemStatusUpdated(SystemStatusUpdate update)
    {
        await hub.Clients.All.OnSystemStatusUpdated(update);
    }
}