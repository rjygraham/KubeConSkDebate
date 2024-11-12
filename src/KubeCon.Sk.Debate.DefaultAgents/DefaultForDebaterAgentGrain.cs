using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Models;
using KubeCon.Sk.Debate.Infrastructure;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace KubeCon.Sk.Debate.DefaultAgents;

public class DefaultForDebaterAgentGrain(Kernel kernel, ILogger<DefaultForDebaterAgentGrain> logger) : AgentGrainBase
{
    private ChatCompletionAgent agent;

    protected override AgentType Role => AgentType.Debater;

    protected override string Instructions => """
    You are a debater in a debate. You have the following goals:
    
    - Debate in favor of the topic provided by the moderator.
    - Always indicate whether you are making an opening argument, rebuttal, or closing argument
    - Always output your supporting points in a numbered list.
    - Never direct the debate towards a new topic.
    - Never direct the debate flow, that is the moderator's job.
    """;
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        agent = new()
        {
            Name = nameof(DefaultModeratorAgentGrain),
            Instructions = Instructions,
            Kernel = kernel,
            Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
        };

        return base.OnActivateAsync(cancellationToken);
    }

    public override async Task InvokeAsync(Guid debateId)
    {
        var debateGrain = GrainFactory.GetGrain<IDebateGrain>(debateId);
        var debate = await debateGrain.GetDebate();

        var history = new ChatHistory();
        var recentMessages = debate.ChatHistory.TakeLast(4);
        foreach (var message in recentMessages)
        {
            history.Add(new ChatMessageContent(AuthorRole.Assistant, message.Content) { AuthorName = message.Author });
        }

        await foreach (ChatMessageContent response in agent.InvokeAsync(history))
        {
            switch (response)
            {
                case OpenAIChatMessageContent aoaiChatMessage:
                    await debateGrain.AddChatMessageAsync(new Abstractions.Models.ChatMessage { Author = nameof(DefaultForDebaterAgentGrain), Content = response.ToString(), Role = ChatMessageAuthorRole.Assistant }, debate.Moderator);
                    break;
                default:
                    break;
            }
        }
    }
}
