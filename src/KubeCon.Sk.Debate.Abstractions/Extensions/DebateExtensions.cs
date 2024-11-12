namespace KubeCon.Sk.Debate.Abstractions.Extensions;

public static class DebateExtensions
{
    public static bool HasAgents(this Models.Debate debate)
    {
        return !(debate.Moderator == null && debate.Debater1 == null && debate.Debater2 == null);
    }

    public static bool IsDebateComplete(this Models.Debate debate)
    {
        return debate.Winner is not null;
    }
}
