using KubeCon.Sk.Debate.Abstractions.Models;
using Orleans.Concurrency;

namespace KubeCon.Sk.Debate.Abstractions;

public interface IAgentSessionGrain : IGrainWithStringKey
{
    [ReadOnly]
    ValueTask<Agent> Get();

    [ReadOnly]
    ValueTask<bool> IsAgentOnline();

    [ReadOnly]
    ValueTask<bool> IsAgentKicked();

    Task SignIn(string agentFullName);

    Task SignOut();

    Task SetOpponent(string opponent);

    Task RecordWin();

    Task RecordLoss();

    Task RecordTie();

    [AlwaysInterleave]
    Task<string> InvokeAsync(Guid debateId);

    Task KickAgent();

    Task UnkickAgent();
}
