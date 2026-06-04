using System.Text.Json; // ← УЖЕ ЕСТЬ, ок
using System.IO;

namespace ToolWeaver.Tools;

public class ReadFileTool : ITool
{
    public string Name => "READ_FILE";
    public string Description => "Прочитать содержимое файла. Формат: {\"tool\": \"READ_FILE\", \"args\": {\"path\": \"config.json\"}}";
    public bool RequiresConfirmation => false;

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            // ← НОВОЕ: Парсим JSON вместо raw-строки
            using var doc = JsonDocument.Parse(args);
            string path = doc.RootElement.GetProperty("path").GetString()?.Trim() ?? string.Empty;

            // ← Твоя проверка на пустоту (без изменений)
            if (string.IsNullOrWhiteSpace(path))
                return "Ошибка: Путь к файлу не указан.";

            path = Path.GetFullPath(path); // ← Было: args.Trim()

            Console.WriteLine(path);
            if (!File.Exists(path))
            {
                return $"Ошибка: Файл '{path}' не найден.";
            }

            string content = await File.ReadAllTextAsync(path);

            if (string.IsNullOrWhiteSpace(content))
                return $"Инфо: Файл '{path}' найден, но он пуст.";

            return $"Успех. Содержимое файла {path}:\n{content}";
        }
        catch (JsonException) // ← НОВОЕ: отлавливаем ошибки парсинга отдельно
        {
            return $"Ошибка формата аргументов: ожидался JSON с полем 'path'.";
        }
        catch (Exception ex)
        {
            return $"Ошибка при чтении: {ex.Message}";
        }
    }
}