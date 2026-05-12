using System.Text.Json; 
namespace ToolWeaver.Tools;

public class ReadFileTool : ITool
{
    public string Name => "READ_FILE";
    public bool RequiresConfirmation => false; 

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            // Десериализация (добавил проверку на ошибки JSON)
            var data = JsonSerializer.Deserialize<WriteFileData>(args);

            if (data == null || string.IsNullOrWhiteSpace(data.FileName))
                return "Ошибка: Имя файла не указано в JSON.";

            
            //Путь файла)
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "Workspace");
            string path = Path.Combine(folder, data.FileName);

            Console.WriteLine(path);
            if (!File.Exists(path))
            {
                return $"Ошибка: Файл '{data.FileName}' не найден в папке Workspace.";
            }

            //Читаем 
            string content = await File.ReadAllTextAsync(path);

            if (string.IsNullOrWhiteSpace(content))
                return $"Инфо: Файл '{data.FileName}' найден, но он пуст.";

            return $"Успех. Содержимое файла {data.FileName}:\n{content}";
        }
        catch (Exception ex)
        {
            return $"Ошибка при чтении: {ex.Message}";
        }
    }
}
