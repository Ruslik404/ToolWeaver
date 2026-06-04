using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace ToolWeaver.Tools;

public class OpenAppTool : ITool
{
    public string Name => "OPEN_APP";
    public string Description => "Открыть приложение или выполнить системную команду. Формат: {\"tool\": \"OPEN_APP\", \"args\": {\"command\": \"имя_программы\"}}";
    public bool RequiresConfirmation => true;

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            using var doc = JsonDocument.Parse(args);
            string command = doc.RootElement.GetProperty("command").GetString()?.Trim() ?? string.Empty; // ← фикс null
    
            if (string.IsNullOrWhiteSpace(command))
                return "Ошибка: поле 'command' не указано.";
    
            Process.Start(command);
    
            return $"Успех: '{command}' запущен в фоне.";
        }
        catch (JsonException) // ← стандартный блок catch
        {
            return "Ошибка формата: нужен JSON {\"command\": \"firefox\"}";
        }
        catch (Exception ex) // ← стандартный блок catch
        {
            return $"Ошибка запуска: {ex.Message}";
        }
    }
}