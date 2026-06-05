using System.Text.Json;

namespace ToolWeaver.Tools;

public class EditFileTool : ITool
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public string Name => "EDIT_FILE";
    public string Description => "Заменяет старый текст на новый внутри файла (поиск и замена).";
    public object Parameters => new { filename = "string", oldText = "string", newText = "string" };
    public bool RequiresConfirmation => true;

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            var data = JsonSerializer.Deserialize<EditFileData>(args, _jsonOptions);
            if (data == null || string.IsNullOrWhiteSpace(data.FileName))
                return "Ошибка: Неверные аргументы.";

            string path = Path.Combine("Workspace", data.FileName);

            if (!File.Exists(path))
                return $"Ошибка: Файл '{data.FileName}' не найден.";

            string content = await File.ReadAllTextAsync(path);

            if (!content.Contains(data.OldText))
                return "Ошибка: Старый текст не найден в файле. Замена невозможна.";

            string newContent = content.Replace(data.OldText, data.NewText);
            await File.WriteAllTextAsync(path, newContent);

            return $"Успех: Файл '{data.FileName}' обновлен.";
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
}
