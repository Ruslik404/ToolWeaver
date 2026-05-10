using System.Text;
using System.Text.Json;
using ToolWeaver.Models;

namespace ToolWeaver.Services;

public class AIService : IDisposable
{
    private readonly HttpClient _client;

    public AIService()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Config.API_KEY}");
    }

    public async Task<string> AskAI(List<Message> history)
    {
        try
        {
            var request = new AIRequest
            {
                Model = Config.MODEL,
                Messages = history.ToArray()
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(Config.API_URL, content);

            // Если OpenRouter вернет ошибку (например, кончились токены), это покажет причину
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                return $"Ошибка API: {response.StatusCode} - {errorDetails}";
            }


            response.EnsureSuccessStatusCode();

            var responseText = await response.Content.ReadAsStringAsync();
            return ExtractAnswer(responseText);
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }

    private string ExtractAnswer(string responseJson)
    {
        // Используем опцию IgnoreCase, так как API иногда меняет регистр полей
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var aiResponse = JsonSerializer.Deserialize<AIResponse>(responseJson, options);
        return aiResponse?.Choices?[0]?.Message?.Content ?? "Нет ответа";
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}
