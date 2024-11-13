using KubeCon.Sk.Debate.Abstractions.Models;

namespace KubeCon.Sk.Debate.Abstractions;

public interface ILeaderboardGrainObserver : IGrainObserver
{
    Task OnDebateTopicSelected(string topic);

    Task OnDebateAgentsSelected(AgentDescriptor moderator, AgentDescriptor debater1, AgentDescriptor debater2);

    Task OnDebateStarted(DateTime startTime);

    Task OnDebateEnded(DateTime endTime);

    Task OnDebateChatMessageAdded(ChatMessage message);

    Task OnLobbyUpdated(List<Agent> agentsInLobby);

    Task OnAgentsOnlineUpdated(List<Agent> agentsOnline);

    Task OnAgentScoresUpdated(Agent agent);

    Task OnSystemStatusUpdated(SystemStatusUpdate update);
}
