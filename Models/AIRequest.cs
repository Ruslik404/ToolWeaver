using System.Text.Json.Serialization;

namespace ToolWeaver.Models;

public class AIRequest
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("messages")]
    public Message[]? Messages { get; set; }


    [JsonPropertyName("temperature")]
    public float Temperature { get; set; } = 0.0f; // Выключает бред и креатив

    [JsonPropertyName("top_p")]
    public float TopP { get; set; } = 0.1f; // Заставляет выбирать только точные слова

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 1500; // Ограничение на длину ответа
}

public class Message
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
