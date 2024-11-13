using KubeCon.Sk.Debate.Abstractions.Models;

namespace KubeCon.Sk.Debate.Abstractions;

public interface ILeaderboardGrain : IGrainWithGuidKey
{
    Task Subscribe(ILeaderboardGrainObserver observer);

    Task UnSubscribe(ILeaderboardGrainObserver observer);

    Task DebateTopicSelected(string topic);

    Task DebateAgentsSelected(AgentDescriptor moderator, AgentDescriptor debater1, AgentDescriptor debater2);

    Task DebateStarted(DateTime startTime);

    Task DebateEnded(DateTime endTime);

    Task DebateChatMessageAdded(ChatMessage message);

    Task LobbyUpdated(List<Agent> agentsInLobby);

    Task AgentsOnlineUpdated(List<Agent> playersOnline);

    Task AgentScoresUpdated(Agent player);

    Task UpdateSystemStatus(SystemStatusUpdate update);
}
