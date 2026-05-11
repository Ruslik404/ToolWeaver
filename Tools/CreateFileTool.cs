namespace ToolWeaver.Tools;

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

            string fileName = args.Trim();
            
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
