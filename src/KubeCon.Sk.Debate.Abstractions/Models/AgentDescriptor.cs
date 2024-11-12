namespace KubeCon.Sk.Debate.Abstractions.Models;

[GenerateSerializer]
public record AgentDescriptor
{
    [Id(0)]
    public string Name { get; set; }

    [Id(1)]
    public string FullName { get; set; }
}
