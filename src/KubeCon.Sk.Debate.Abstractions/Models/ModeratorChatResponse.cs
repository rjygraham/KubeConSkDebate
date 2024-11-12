using System.Text.Json.Serialization;

namespace KubeCon.Sk.Debate.Abstractions.Models;

public class ModeratorChatResponse
{
    [JsonPropertyName("nextAgent")]
    public string NextAgent { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}
