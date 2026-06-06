using System.Text.Json; // ← УЖЕ ЕСТЬ, ок
using System.IO;

namespace ToolWeaver.Tools;

public class ReadFileTool : ITool
{
    public string Name => "READ_FILE";
    public string Description => "Прочитать содержимое файла.";
    public object Parameters => new { path = "string" };
    public bool RequiresConfirmation => false;

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            using var doc = JsonDocument.Parse(args);
            string fileName = doc.RootElement.GetProperty("path").GetString()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(fileName))
                return "Ошибка: Путь к файлу не указан.";

            // Работаем строго внутри папки Workspace
            string path = Path.Combine("Workspace", fileName);

            if (!File.Exists(path))
            {
                return $"Ошибка: Файл '{fileName}' не найден в папке Workspace.";
            }

            string content = await File.ReadAllTextAsync(path);
            return content;
        }
        catch (JsonException)
        {
            return "Ошибка формата: ожидался JSON с полем 'path'.";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }
}