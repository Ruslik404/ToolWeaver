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
            if (string.IsNullOrWhiteSpace(args))
                return "Ошибка: Путь к файлу не указан.";

            string path = Path.GetFullPath(args.Trim());

            Console.WriteLine(path);
            if (!File.Exists(path))
            {
                return $"Ошибка: Файл '{path}' не найден в папке Workspace.";
            }

            //Читаем 
            string content = await File.ReadAllTextAsync(path);

            if (string.IsNullOrWhiteSpace(content))
                return $"Инфо: Файл '{path}' найден, но он пуст.";

            return $"Успех. Содержимое файла {path}:\n{content}";
        }
        catch (Exception ex)
        {
            return $"Ошибка при чтении: {ex.Message}";
        }
    }
}
