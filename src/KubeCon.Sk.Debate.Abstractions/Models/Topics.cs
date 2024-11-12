namespace KubeCon.Sk.Debate.Abstractions.Models;

[GenerateSerializer]
public class Topics
{
    [Id(0)]
    public List<string> Available { get; set; } = new();

    [Id(1)]
    public List<string> PreviouslyUsed { get; set; } = new();
}
