namespace KubeCon.Sk.Debate.Abstractions.Models;

[GenerateSerializer]
public class Debate
{
    [Id(0)]
    public Guid Id { get; set; } = Guid.Empty;

    [Id(1)]
    public string Topic { get; set; } = string.Empty;

    [Id(2)]
    public List<ChatMessage> ChatHistory { get; set; } = new();

    [Id(3)]
    public DateTime Started { get; set; } = DateTime.MinValue;

    [Id(4)]
    public DateTime Ended { get; set; } = DateTime.MinValue;

    [Id(5)]
    public AgentDescriptor? Moderator { get; set; }

    [Id(6)]
    public AgentDescriptor? Debater1 { get; set; }

    [Id(7)]
    public AgentDescriptor? Debater2 { get; set; }

    [Id(8)]
    public AgentDescriptor? NextAgent { get; set; }

    [Id(9)]
    public string? Winner { get; set; }

    public bool IsReady => !string.IsNullOrWhiteSpace(Topic) && Moderator != null && Debater1 != null && Debater2 != null;

    public bool IsComplete => !string.IsNullOrEmpty(Winner) && Ended != DateTime.MinValue;
}
