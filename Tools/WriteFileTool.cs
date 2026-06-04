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
    public string Description => "Записать текст в файл. ВАЖНО: args должен содержать 'filename' и 'content'. Формат: {\"tool\": \"WRITE_FILE\", \"args\": {\"filename\": \"имя.txt\", \"content\": \"текст\"}}";
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
            
            // Безопасная сборка пути: предотвращаем выход за пределы папки Workspace
            string fileName = Path.GetFileName(data.FileName); 
            string path = Path.Combine(folder, fileName);

            await File.WriteAllTextAsync(path, data.Content);
            return $"Успех: Текст записан в файл '{fileName}' в папке Workspace.";
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