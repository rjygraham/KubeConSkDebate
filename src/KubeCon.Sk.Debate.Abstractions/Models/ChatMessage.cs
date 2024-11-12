namespace KubeCon.Sk.Debate.Abstractions.Models;

[GenerateSerializer]
public class ChatMessage
{
    [Id(0)]
    public string Author { get; set; }

    [Id(1)]
    public string Content { get; set; }

    [Id(2)]
    public ChatMessageAuthorRole Role { get; set; }

}
