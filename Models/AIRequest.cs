using System.Text.Json.Serialization;

namespace ToolWeaver.Models;

public class AIRequest
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("messages")]
    public Message[]? Messages { get; set; }
}

public class Message
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
