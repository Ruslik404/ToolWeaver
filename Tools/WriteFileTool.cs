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
    public string Description => "Записывает текст в файл.";
    public object Parameters => new { filename = "string", content = "string" };
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
            string path = Path.Combine(folder, data.FileName);
            
            if (!File.Exists(path))
            {
                // Проверяем, существует ли хотя бы директория
                string? directory = Path.GetDirectoryName(path);
                if (directory != null && !Directory.Exists(directory))
                {
                    return $"Ошибка: Директория '{directory}' не существует. Сначала создайте её через CREATE_FILE.";
                }
            }

            await File.WriteAllTextAsync(path, data.Content);
            return $"Успех: Файл '{data.FileName}' записан.";
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