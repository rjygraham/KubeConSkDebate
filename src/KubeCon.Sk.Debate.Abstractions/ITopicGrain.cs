namespace KubeCon.Sk.Debate.Abstractions;

public interface ITopicGrain : IGrainWithGuidKey
{
    Task<string> GetNextTopicAsync();

    Task AddTopicAsync(string topic);
}
