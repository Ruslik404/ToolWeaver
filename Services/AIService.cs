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

    public async Task<string> AskAI(string question)
    {
        try
        {
            var request = new AIRequest
            {
                Model = Config.MODEL,
                Messages = new[]
                {
                    new Message { Role = "user", Content = question }
                }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(Config.API_URL, content);

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
        var aiResponse = JsonSerializer.Deserialize<AIResponse>(responseJson);
        return aiResponse?.Choices?[0]?.Message?.Content ?? "Нет ответа";
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}
