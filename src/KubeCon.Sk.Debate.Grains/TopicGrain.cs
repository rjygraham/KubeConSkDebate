using KubeCon.Sk.Debate.Abstractions;
using KubeCon.Sk.Debate.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Chat;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KubeCon.Sk.Debate.Grains;

public class TopicGrain(
    [PersistentState("Topics", storageName: "Topics")] IPersistentState<Topics> topics,
    IServiceProvider serviceProvider
) : ITopicGrain
{
    public IPersistentState<Topics> Topics { get; } = topics;

    public async Task AddTopicAsync(string topic)
    {
        Topics.State.Available.Insert(0, topic);
        await Topics.WriteStateAsync();
    }

    public async Task<string> GetNextTopicAsync()
    {
        if (Topics.State.Available.Count < 5)
        {
            var kernel = serviceProvider.GetRequiredService<Kernel>();
            var chat = kernel.GetRequiredService<IChatCompletionService>();
            var result = await chat.GetChatMessageContentAsync("Provide a list of 10 fun debate topics that can be debated either for or against. Respond with following json format: { \"topics\": [\"topic 1\", \"topic 2\"] }");
            var json = ((ChatCompletion)result.InnerContent).Content[0].Text;

            var newTopics = JsonSerializer.Deserialize<TopicsResponse>(json.Trim("```").TrimStart("json"));

            Topics.State.Available.AddRange(newTopics.Topics);
        }

        var topic = Topics.State.Available.First();
        Topics.State.Available.Remove(topic);
        Topics.State.PreviouslyUsed.Add(topic);
        await Topics.WriteStateAsync();

        return topic;
    }

    private class TopicsResponse
    {
        [JsonPropertyName("topics")]
        public string[] Topics { get; set; }
    }
}
