using System.Text.Json.Serialization;

namespace ToolWeaver.Models;

public class AIResponse
{
    [JsonPropertyName("choices")]
    public Choice[]? Choices { get; set; }
}

public class Choice
{
    [JsonPropertyName("message")]
    public MessageContent? Message { get; set; }
}

public class MessageContent
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
