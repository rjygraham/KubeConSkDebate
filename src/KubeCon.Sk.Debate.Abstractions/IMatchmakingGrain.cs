using KubeCon.Sk.Debate.Abstractions.Models;

namespace KubeCon.Sk.Debate.Abstractions;

public interface IMatchmakingGrain : IGrainWithGuidKey
{
    Task<(Agent moderator, Agent debater1, Agent debater2)?> ChooseAgents();
}