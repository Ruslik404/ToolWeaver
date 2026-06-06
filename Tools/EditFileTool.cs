using System.Text.Json;
using System.Text.RegularExpressions;
using ToolWeaver; // ИСПРАВЛЕНО 1: Изменено с ToolWeaver.Interfaces на ToolWeaver

namespace ToolWeaver.Tools;

public class EditFileTool : ITool
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public string Name => "EDIT_FILE";
    
    public string Description => "Редактирует файл, заменяя старый текст на новый. " +
                                 "Передайте occurrenceNumber: 0 (заменить все) или 1, 2, 3... (заменить конкретное вхождение).";

    // ИСПРАВЛЕНО 2: Реализовано обязательное свойство Parameters для интерфейса ITool
    public object Parameters => new 
    { 
        filename = "string", 
        oldText = "string", 
        newText = "string", 
        occurrenceNumber = "number" 
    };

    // ИСПРАВЛЕНО 3: Реализовано обязательное свойство RequiresConfirmation для интерфейса ITool
    // Замена кода — критическое действие, поэтому запрашиваем подтверждение пользователя (true)
    public bool RequiresConfirmation => true; 

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            var data = JsonSerializer.Deserialize<EditFileData>(args, _jsonOptions);
            if (data == null || string.IsNullOrWhiteSpace(data.FileName) || string.IsNullOrEmpty(data.OldText))
                return "Ошибка: Неверные аргументы.";

            // Защита и привязка к папке Workspace
            string workspaceRoot = Path.GetFullPath("Workspace");
            if (!Directory.Exists(workspaceRoot)) Directory.CreateDirectory(workspaceRoot);
            string safePath = Path.GetFullPath(Path.Combine(workspaceRoot, data.FileName));

            if (!safePath.StartsWith(workspaceRoot, StringComparison.OrdinalIgnoreCase))
                return "Ошибка: Отказано в доступе за пределы Workspace.";

            if (!File.Exists(safePath))
                return $"Ошибка: Файл '{data.FileName}' не найден.";

            string content = await File.ReadAllTextAsync(safePath);

            // Лаконичное решение с помощью Regex
            string pattern = Regex.Escape(data.OldText);
            int matchCount = 0;
            bool isReplaced = false;

            string newContent = Regex.Replace(content, pattern, m =>
            {
                matchCount++;
                if (data.OccurrenceNumber <= 0 || matchCount == data.OccurrenceNumber)
                {
                    isReplaced = true;
                    return data.NewText;
                }
                return m.Value;
            });

            if (!isReplaced)
                return $"Ошибка: Текст не найден. Всего совпадений в файле: {matchCount}.";

            await File.WriteAllTextAsync(safePath, newContent);
            return $"Успех: Файл обновлен.";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }
}

public class EditFileData
{
    public string FileName { get; set; } = string.Empty;
    public string OldText { get; set; } = string.Empty;
    public string NewText { get; set; } = string.Empty;
    public int OccurrenceNumber { get; set; } = 1;
}
