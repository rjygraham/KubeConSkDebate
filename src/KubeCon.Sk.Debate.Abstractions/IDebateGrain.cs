using KubeCon.Sk.Debate.Abstractions.Models;
using Orleans.Concurrency;

namespace KubeCon.Sk.Debate.Abstractions;

public interface IDebateGrain : IGrainWithGuidKey
{
    [ReadOnly]
    ValueTask<Models.Debate> GetDebate();

    Task SelectTopicAsync();

    Task SelectAgentsAsync();

    Task StartDebateAsync();

    [AlwaysInterleave]
    Task DebateAsync();

    Task AddChatMessageAsync(ChatMessage message, AgentDescriptor nextAgent);

    Task EndDebateAsync(ChatMessage message);

    Task SelectWinnerAsync(string agentName);
}
