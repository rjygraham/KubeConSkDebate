using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Models;
using Microsoft.Extensions.Logging;
using Orleans.Utilities;

namespace KubeCon.Sk.Debate.Grains;

public class LeaderboardGrain(ILogger<LeaderboardGrain> logger) : Grain, ILeaderboardGrain
{
    private readonly ObserverManager<ILeaderboardGrainObserver> observers = new(TimeSpan.FromMinutes(1), logger);

    public Task Subscribe(ILeaderboardGrainObserver observer)
    {
        observers.Subscribe(observer, observer);
        return Task.CompletedTask;
    }

    public async Task DebateTopicSelected(string topic)
    {
        await observers.Notify(observer => observer.OnDebateTopicSelected(topic));
    }
    public async Task DebateAgentsSelected(AgentDescriptor moderator, AgentDescriptor debater1, AgentDescriptor debater2)
    {
        await observers.Notify(observer => observer.OnDebateAgentsSelected(moderator, debater1, debater2));
    }

    public async Task DebateStarted(DateTime startTime)
    {
        await observers.Notify(observer => observer.OnDebateStarted(startTime));
    }

    public async Task DebateChatMessageAdded(ChatMessage message)
    {
        await observers.Notify(observer => observer.OnDebateChatMessageAdded(message));
    }

    public async Task DebateEnded(DateTime endTime)
    {
        await observers.Notify(observer => observer.OnDebateEnded(endTime));
    }

    public Task UnSubscribe(ILeaderboardGrainObserver observer)
    {
        observers.Unsubscribe(observer);
        return Task.CompletedTask;
    }

    public async Task LobbyUpdated(List<Agent> agentsInLobby)
    {
        await observers.Notify(observer => observer.OnLobbyUpdated(agentsInLobby));
    }

    public async Task AgentsOnlineUpdated(List<Agent> agentsOnline)
    {
        List<Agent> orderedAgents = [.. agentsOnline.OrderByDescending(x => x.PercentWon)];
        await observers.Notify(observer => observer.OnAgentsOnlineUpdated(orderedAgents));
    }

    public async Task AgentScoresUpdated(Agent agent)
    {
        await observers.Notify(observer => observer.OnAgentScoresUpdated(agent));
    }

    public async Task UpdateSystemStatus(SystemStatusUpdate update)
    {
        await observers.Notify(observer => observer.OnSystemStatusUpdated(update));
    }

   
}