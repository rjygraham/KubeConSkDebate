using KubeCon.Sk.Debate.Abstractions;
using System.Diagnostics;

namespace KubeCon.Sk.Debate.Host;

public class DebateEngineStateController(IGrainFactory grainFactory)
{
    private IGrainFactory _grainFactory = grainFactory;

    public bool IsStarted { get; private set; } = true;
    public int DebateLoopDelayInMilliseconds { get; private set; } = 5000;
    public int DebatesCompleted { get; private set; } = 0;
    public DateTime ServerStartedTime { get; private set; } = DateTime.UtcNow;
    public Stopwatch Uptime { get; private set; } = Stopwatch.StartNew();
    public void Start() => IsStarted = true;
    public void Stop() => IsStarted = false;
    public void SetDebateLoopDelayInterval(int debateLoopDelayInMilliseconds) => DebateLoopDelayInMilliseconds = debateLoopDelayInMilliseconds;
    public void IncrementDebateCount() => DebatesCompleted++;
    public async Task KickAgent(string agentName) => await _grainFactory.GetGrain<IAgentSessionGrain>(agentName).KickAgent();
    public async Task UnkickAgent(string agentName) => await _grainFactory.GetGrain<IAgentSessionGrain>(agentName).UnkickAgent();
    public async Task AddTopic(string topic) => await _grainFactory.GetGrain<ITopicGrain>(Guid.Empty).AddTopicAsync(topic);
}
