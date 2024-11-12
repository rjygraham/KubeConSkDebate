using KubeCon.Sk.Debate.Abstractions.Models;
using Orleans.Concurrency;

namespace KubeCon.Sk.Debate.Abstractions;

public interface IAgentGrain : IGrainWithStringKey
{
    [ReadOnly]
    ValueTask<AgentType> GetRole();

    [ReadOnly]
    ValueTask<string> GetInstructions();

    [AlwaysInterleave]
    Task InvokeAsync(Guid debateId);
}
