namespace KubeCon.Sk.Debate.Abstractions.Models;

[GenerateSerializer]
public class Agent
{
    [Id(0)]
    public string? Name { get; set; }

    [Id(1)]
    public string? FullName { get; set; }

    [Id(2)]
    public AgentType AgentType { get; set; }

    [Id(3)]
    public string Instructions { get; set; } = string.Empty;

    [Id(4)]
    public string CurrentOpponent { get; set; } = string.Empty;

    [Id(5)]
    public int TotalDebates { get; set; }

    [Id(6)]
    public int WinCount { get; set; }

    [Id(7)]
    public int LossCount { get; set; }

    [Id(8)]
    public int TieCount { get; set; }

    [Id(9)]
    public int PercentWon { get; set; }

    [Id(10)]
    public bool IsActive { get; set; }

    [Id(11)]
    public bool IsKicked { get; set; }

    public override bool Equals(object? obj) => obj is Agent agent && Name == agent.Name;

    public override int GetHashCode() => HashCode.Combine(Name);

}
