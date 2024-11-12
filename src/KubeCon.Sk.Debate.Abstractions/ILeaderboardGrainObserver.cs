using KubeCon.Sk.Debate.Abstractions.Models;

namespace KubeCon.Sk.Debate.Abstractions;

public interface ILeaderboardGrainObserver : IGrainObserver
{
    Task OnDebateStarted(Models.Debate debate);

    Task OnDebateCompleted(Models.Debate debate);

    Task OnDebateChatMessageAdded(ChatMessage message);

    Task OnLobbyUpdated(List<Agent> agentsInLobby);

    Task OnAgentsOnlineUpdated(List<Agent> agentsOnline);

    Task OnAgentScoresUpdated(Agent agent);

    Task OnSystemStatusUpdated(SystemStatusUpdate update);
}
