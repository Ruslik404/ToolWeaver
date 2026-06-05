namespace ToolWeaver.Tools;
using System.Text.Json;
public class CreateFileTool : ITool
{
    public string Name => "CREATE_FILE";
    public string Description => "Создать пустой файл. Автоматически создает папки, если они указаны в пути (например, 'folder/file.txt').";
    public object Parameters => new { filename = "string" };
    public bool RequiresConfirmation => true;

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            var data = JsonSerializer.Deserialize<CreateFileData>(args, _jsonOptions);
            if (data == null || string.IsNullOrWhiteSpace(data.FileName))
                return "Ошибка: Неверный формат JSON или пустое имя файла.";

            string folder = "Workspace";
            string path = Path.Combine(folder, data.FileName);

            // Автоматически создаем все папки по указанному пути
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(path))
                return $"Ошибка: Файл '{data.FileName}' уже существует.";

            await File.WriteAllTextAsync(path, "");
            return $"Успех: Файл '{data.FileName}' создан (включая необходимые директории).";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}

public class CreateFileData
{
    public string FileName { get; set; } = string.Empty;
}
