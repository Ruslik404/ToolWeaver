using System.Text.Json; 
using System.IO;
using System.Linq;

namespace ToolWeaver.Tools;

public class ListFilesTool : ITool
{
    public string Name => "LIST_FILES";
    public string Description => "Узнать список файлов в папке. Если путь не передан, используется './Workspace'. Формат: {\"tool\": \"LIST_FILES\", \"args\": {\"path\": \"./docs\"}}";
    public bool RequiresConfirmation => false;

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            //Парсим JSON вместо raw-строки
            using var doc = JsonDocument.Parse(args);
            string folder = doc.RootElement.GetProperty("path").GetString()?.Trim() ?? string.Empty;


            if (string.IsNullOrWhiteSpace(folder))
                folder = Path.Combine(Directory.GetCurrentDirectory(), "Workspace");
            else
                folder = Path.GetFullPath(folder);


            var files = Directory.EnumerateFileSystemEntries(folder);
            string result = string.Join(", ", files.Select(Path.GetFileName));

            if (string.IsNullOrEmpty(result))
                return $"Папка '{Path.GetFileName(folder)}' пуста.";

            return $"Успех. Файлы: {result}";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }
}