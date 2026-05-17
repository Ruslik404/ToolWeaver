namespace ToolWeaver.Tools;
using System.Text.Json;
public class CreateFileTool : ITool
{
    public string Name => "CREATE_FILE";
    public bool RequiresConfirmation => true; 

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            string folder = "Workspace";
            Directory.CreateDirectory(folder); 
            using var doc = JsonDocument.Parse(args);
            // Замени строку с fileName на:
            string fileName = doc.RootElement.GetProperty("filename").GetString()?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(fileName)) return "Ошибка: имя файла не указано.";

            string path = Path.Combine(folder, fileName); //полный путь

            await File.WriteAllTextAsync(path, string.Empty); // Создаем пустой файл
            return $"Успех: Пустой файл '{fileName}' создан.";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }
}
