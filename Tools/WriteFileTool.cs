using System.Text.Json; 
namespace ToolWeaver.Tools;

public class WriteFileTool : ITool
{
    public string Name => "WRITE_FILE";
    public bool RequiresConfirmation => true; 

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            // Пытаемся превратить JSON-строку в объект
            var data = JsonSerializer.Deserialize<WriteFileData>(args);

            if (data == null || string.IsNullOrWhiteSpace(data.FileName))
                return "Ошибка: Неверный формат JSON или пустое имя файла.";

            string folder = "Workspace";
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, data.FileName);

            await File.WriteAllTextAsync(path, data.Content);
            return $"Успех: Текст записан в файл {data.FileName}.";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }
}

// Маленький вспомогательный класс для десериализации
public class WriteFileData
{
    public string FileName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
