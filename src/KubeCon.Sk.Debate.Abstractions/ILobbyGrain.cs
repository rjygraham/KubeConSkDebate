using KubeCon.Sk.Debate.Abstractions.Models;
using Orleans.Concurrency;

namespace KubeCon.Sk.Debate.Abstractions;

public interface ILobbyGrain : IGrainWithGuidKey
{
    [ReadOnly]
    ValueTask<List<Agent>> GetAgentsInLobby();

    Task SignIn(Agent agent);

    Task SignOut(Agent agent);

    Task EnterLobby(Agent agent);

    Task EnterDebate(Agent agent);
}
