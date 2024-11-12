using KubeCon.Sk.Debate.Abstractions.Models;
using Orleans.Concurrency;

namespace KubeCon.Sk.Debate.Abstractions;

public interface IDebateGrain : IGrainWithGuidKey
{
    [ReadOnly]
    ValueTask<Models.Debate> GetDebate();

    Task SelectAgents();

    Task SetDebate(Models.Debate debate);

    [AlwaysInterleave]
    Task Go();

    Task AddChatMessageAsync(ChatMessage message, AgentDescriptor nextAgent);

    Task EndDebate(ChatMessage message);

    Task SelectWinner(string agentName);
}
