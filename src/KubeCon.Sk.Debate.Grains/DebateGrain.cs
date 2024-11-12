using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace KubeCon.Sk.Debate.Grains;

[CollectionAgeLimit(Minutes = 2)]
public class DebateGrain(
    [PersistentState("Debates", storageName: "Debates")] IPersistentState<Abstractions.Models.Debate> debate,
    ILogger<DebateGrain> logger
) : Grain, IDebateGrain
{
    private IPersistentState<Abstractions.Models.Debate> Debate { get; set; } = debate;
    private Dictionary<string, IAgentSessionGrain> debateAgents = new();

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Debate.State.Id = this.GetPrimaryKey();

        return base.OnActivateAsync(cancellationToken);
    }

    public ValueTask<Abstractions.Models.Debate> GetDebate() => ValueTask.FromResult(Debate.State);

    public Task SetDebate(Abstractions.Models.Debate debate) => Task.FromResult(Debate.State = debate);

    public async Task SelectAgents()
    {
        logger.LogInformation("SkD: Getting the matchmaker for debate {DebateId}", Debate.State.Id);
        var matchmaker = GrainFactory.GetGrain<IMatchmakingGrain>(Guid.Empty);

        logger.LogInformation("SkD: Getting agents for Debate {DebateId}", Debate.State.Id);
        var chosenAgents = await matchmaker.ChooseAgents();

        if (chosenAgents is not var (moderator, debater1, debater2))
        {
            logger.LogInformation("SkD: There aren't enough agents in the lobby to field a debate.");
        }
        else
        {
            logger.LogInformation("SkD: Agents {AgentOneName} and {AgentTwoName} selected for Debate {DebateId}.", debater1.Name, debater2.Name, Debate.State.Id);

            // start the debate
            Debate.State.Moderator = new AgentDescriptor { Name = moderator.Name, FullName = moderator.FullName };
            Debate.State.Debater1 = new AgentDescriptor { Name = debater1.Name, FullName = debater1.FullName };
            Debate.State.Debater2 = new AgentDescriptor { Name = debater2.Name, FullName = debater2.FullName };

            debateAgents.Add(Debate.State.Moderator.Name, GrainFactory.GetGrain<IAgentSessionGrain>(Debate.State.Moderator.Name));
            debateAgents.Add(Debate.State.Debater1.Name, GrainFactory.GetGrain<IAgentSessionGrain>(Debate.State.Debater1.Name));
            debateAgents.Add(Debate.State.Debater2.Name, GrainFactory.GetGrain<IAgentSessionGrain>(Debate.State.Debater2.Name));

            Debate.State.Started = DateTime.UtcNow;
            await Debate.WriteStateAsync();

            // Notify the leaderboard
            var leaderBoard = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
            await leaderBoard.DebateStarted(Debate.State);
        }
    }

    public async Task Go()
    {
        if (!await debateAgents[Debate.State.Moderator.Name].IsAgentOnline()
            || !await debateAgents[Debate.State.Debater1.Name].IsAgentOnline()
            || !await debateAgents[Debate.State.Debater2.Name].IsAgentOnline())
        {
            return;
        }

        if (string.IsNullOrEmpty(Debate.State.Topic))
        {
            var topic = await GrainFactory.GetGrain<ITopicGrain>(Guid.Empty).GetNextTopicAsync();
            Debate.State.Topic = topic;
            Debate.State.ChatHistory.Add(new ChatMessage { Author = "User", Content = topic, Role = ChatMessageAuthorRole.User });
            Debate.State.NextAgent = Debate.State.Moderator;
            await Debate.WriteStateAsync();
        }

        await debateAgents[Debate.State.NextAgent.Name].InvokeAsync(Debate.State.Id);
    }

    public async Task SelectWinner(string agentName)
    {
        var debater1Grain = GrainFactory.GetGrain<IAgentSessionGrain>(Debate.State.Debater1.Name);
        var debater2Grain = GrainFactory.GetGrain<IAgentSessionGrain>(Debate.State.Debater2.Name);
        var debater1 = await debater1Grain.Get();
        var debater2 = await debater2Grain.Get();

        if (agentName.Equals(Debate.State.Debater1.Name))
        {
            await debater1Grain.RecordWin();
            await debater2Grain.RecordLoss();
            logger.LogInformation("SkD: {DebaterName} wins.", debater1.Name);
        }
        else if (agentName.Equals(Debate.State.Debater2.Name))
        {
            await debater2Grain.RecordWin();
            await debater1Grain.RecordLoss();
            logger.LogInformation("SkD: {DebaterName} wins.", debater2.Name);
        }
        else
        {
            await debater2Grain.RecordTie();
            await debater1Grain.RecordTie();
            logger.LogInformation("SkD: {Debater1Name} ties with {Debater2Name}.", debater1.Name, debater2.Name);
        }

        await SetDebate(Debate.State);

        var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);

        if (await debater1Grain.IsAgentOnline())
            await lobbyGrain.EnterLobby(debater1);

        if (await debater2Grain.IsAgentOnline())
            await lobbyGrain.EnterLobby(debater2);

        var leaderBoard = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
        await leaderBoard.DebateCompleted(Debate.State);
    }

    public async Task AddChatMessageAsync(ChatMessage message, AgentDescriptor nextAgent)
    {
        Debate.State.ChatHistory.Add(message);
        Debate.State.NextAgent = nextAgent;
        await Debate.WriteStateAsync();

        var leaderBoard = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
        await leaderBoard.DebateChatMessageAdded(message);
    }

    public async Task EndDebate(ChatMessage message)
    {
        await AddChatMessageAsync(message, new AgentDescriptor { Name = "end" });

        var leaderBoard = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
        await leaderBoard.DebateCompleted(Debate.State);
    }
}