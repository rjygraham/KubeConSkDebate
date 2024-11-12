using KubeCon.Sk.Debate.Abstractions.Models;

namespace KubeCon.Sk.Debate.Abstractions;

public interface ILeaderboardGrain : IGrainWithGuidKey
{
    Task DebateStarted(Models.Debate debate);

    Task DebateCompleted(Models.Debate debate);

    Task DebateChatMessageAdded(ChatMessage message);

    Task Subscribe(ILeaderboardGrainObserver observer);

    Task UnSubscribe(ILeaderboardGrainObserver observer);

    Task LobbyUpdated(List<Agent> agentsInLobby);

    Task AgentsOnlineUpdated(List<Agent> playersOnline);

    Task AgentScoresUpdated(Agent player);

    Task UpdateSystemStatus(SystemStatusUpdate update);
}
