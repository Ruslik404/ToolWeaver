using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace ToolWeaver.Tools;

public class DesktopNotifyTool : ITool
{
    public string Name => "DESKTOP_NOTIFY";
    public bool RequiresConfirmation => false; 

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            using var doc = JsonDocument.Parse(args);
            string title = doc.RootElement.GetProperty("title").GetString()?.Trim() ?? "ToolWeaver";
            string message = doc.RootElement.GetProperty("message").GetString()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(message))
                return "Ошибка: поле 'message' обязательно.";

            var psi = new ProcessStartInfo("notify-send")
            {
                UseShellExecute = false,
                RedirectStandardError = true
            };
            psi.ArgumentList.Add(title);
            psi.ArgumentList.Add(message);

            using var process = Process.Start(psi);
            if (process == null) return "Ошибка: не удалось запустить службу уведомлений."; 
            await process.WaitForExitAsync();

            return process.ExitCode == 0 
                ? $"Успех: Уведомление '{title}' отправлено." 
                : $"Ошибка: уведомление не доставлено.";
        }
        catch (JsonException)
        {
            return "Ошибка формата: нужен JSON {\"title\": \"Заголовок\", \"message\": \"Текст\"}";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }
}