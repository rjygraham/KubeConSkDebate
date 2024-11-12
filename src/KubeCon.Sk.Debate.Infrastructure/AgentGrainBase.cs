using KubeCon.Sk.Debate.Abstractions.Models;
using KubeCon.Sk.Debate.Abstractions;

namespace KubeCon.Sk.Debate.Infrastructure;

public abstract class AgentGrainBase : Grain, IAgentGrain
{
    protected abstract AgentType Role { get; }

    protected abstract string Instructions { get; }

    public ValueTask<AgentType> GetRole() => ValueTask.FromResult(Role);

    public ValueTask<string> GetInstructions() => ValueTask.FromResult(Instructions);

    public abstract Task InvokeAsync(Guid debateId);
}
