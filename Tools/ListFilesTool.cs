namespace ToolWeaver.Tools;

public class ListFilesTool : ITool
{
    public string Name => "LIST_FILES";
    public bool RequiresConfirmation => false; 

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "Workspace");
            
            Directory.CreateDirectory(folder);
            Console.WriteLine(folder);

            var files = Directory.EnumerateFileSystemEntries(folder);
            string result = string.Join(", ", files.Select(Path.GetFileName));

            if (string.IsNullOrEmpty(result))
                return "Папка Workspace пуста.";

            return $"Успех. Файлы: {result}";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }
}
