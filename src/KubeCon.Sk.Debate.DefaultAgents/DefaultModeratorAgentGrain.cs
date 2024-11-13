using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Models;
using KubeCon.Sk.Debate.Infrastructure;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;

namespace KubeCon.Sk.Debate.DefaultAgents;

public class DefaultModeratorAgentGrain(Kernel kernel, ILogger<DefaultModeratorAgentGrain> logger) : AgentGrainBase
{
    private ChatCompletionAgent agent;

    protected override AgentType Role => AgentType.Moderator;

    protected override string Instructions => """
    You are a debate moderator. You have the following goals:
    
    - Control the flow of the debate - direct the next invocation to either Debater1Agent or Debater2Agent.
    - Only respond in the following JSON format: { "nextAgent": "AgentName", "message": "message content should also include the AgentName" }
    - Start the debate by flipping a coin to determine which debater will go first - heads for Debater1Agent, tails for Debater2Agent.
    - Ensure each debater has an opportunity to make an opening argument.
    - Ensure each debater has an opportunity to make a rebuttal.
    - Ensure each debater has an opportunity to make a closing argument.
    - After each debater has made closing arguments, only respond in the following JSON format: { "nextAgent": "end", "message": "A brief remark on the debate asking the end user to select a winner." }
    - Do not allow the debaters to suggest another debate topic.
    """;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var executionSettings = new AzureOpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(), ResponseFormat = "json_object" };
        agent = new()
        {
            Name = nameof(DefaultModeratorAgentGrain),
            Instructions = Instructions,
            Kernel = kernel,
            Arguments = new KernelArguments(executionSettings)
        };

        return base.OnActivateAsync(cancellationToken);
    }

    public override async Task InvokeAsync(Guid debateId)
    {
        var debateGrain = GrainFactory.GetGrain<IDebateGrain>(debateId);
        var debate = await debateGrain.GetDebate();

        var history = new ChatHistory();
        history.AddSystemMessage($"Refer to Debater1Agent as {debate.Debater1.Name} and Debater2Agent as {debate.Debater2.Name}");
        var recentMessages = debate.ChatHistory.TakeLast(4);
        foreach (var message in recentMessages)
        {
            history.Add(new ChatMessageContent(AuthorRole.Assistant, message.Content) { AuthorName = message.Author });
        }

        await foreach (ChatMessageContent response in agent.InvokeAsync(history))
        {
            switch (response)
            {
                case OpenAIChatMessageContent message:
                    var chatResponse = JsonSerializer.Deserialize<ModeratorChatResponse>(response.ToString());

                    if (chatResponse.NextAgent.Equals("end"))
                    {
                        await debateGrain.EndDebateAsync(
                            new ChatMessage
                            {
                                Author = nameof(DefaultModeratorAgentGrain),
                                Content = chatResponse.Message,
                                Role = ChatMessageAuthorRole.Assistant
                            }
                        );
                    }
                    else
                    {
                        await debateGrain.AddChatMessageAsync(
                           new ChatMessage
                           {
                               Author = nameof(DefaultModeratorAgentGrain),
                               Content = chatResponse.Message,
                               Role = ChatMessageAuthorRole.Assistant
                           },
                           GetNextAgent(debate, chatResponse.NextAgent)
                       );
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private AgentDescriptor GetNextAgent(Abstractions.Models.Debate debate, string nextAgent)
    {
        return debate.Debater1.Name.Equals(nextAgent)
            ? debate.Debater1
            : debate.Debater2;
    }
}
