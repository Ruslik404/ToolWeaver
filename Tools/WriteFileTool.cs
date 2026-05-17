using System.Text.Json; 
namespace ToolWeaver.Tools;

public class WriteFileTool : ITool
{
    // ← НОВОЕ: Опции для игнорирования регистра имён свойств
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public string Name => "WRITE_FILE";
    public bool RequiresConfirmation => true; 

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            // ← ИЗМЕНЕНО: Передаём опции в десериализатор
            var data = JsonSerializer.Deserialize<WriteFileData>(args, _jsonOptions);

            if (data == null || string.IsNullOrWhiteSpace(data.FileName))
                return "Ошибка: Неверный формат JSON или пустое имя файла.";

            string folder = "Workspace";
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, data.FileName);

            await File.WriteAllTextAsync(path, data.Content);
            return $"Успех: Текст записан в файл '{data.FileName}'.";
        }
        catch (JsonException) // ← НОВОЕ: ловим ошибки формата отдельно
        {
            return $"Ошибка формата: ожидался JSON с полями 'filename' и 'content'.";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }
}

public class WriteFileData
{
    public string FileName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}