namespace KubeCon.Sk.Debate.Abstractions.Models;

[GenerateSerializer]
public class SystemStatusUpdate
{
    [Id(0)]
    public DateTime DateStarted { get; set; }

    [Id(1)]
    public int GrainsActive { get; set; }

    [Id(2)]
    public TimeSpan TimeUp { get; set; }

    [Id(3)]
    public int DebatesCompleted { get; set; }
}
