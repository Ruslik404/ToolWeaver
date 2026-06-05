using System.Text.Json;

namespace ToolWeaver.Tools;

public class CreateToolTool : ITool
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public string Name => "CREATE_NEW_TOOL";
    public string Description => "Создать новый инструмент (C# класс).";
    public object Parameters => new { className = "string", code = "string" };
    public bool RequiresConfirmation => true;

    public async Task<string> ExecuteAsync(string args)
    {
        try
        {
            var data = JsonSerializer.Deserialize<CreateToolData>(args, _jsonOptions);

            if (data == null || string.IsNullOrWhiteSpace(data.ClassName) || string.IsNullOrWhiteSpace(data.Code))
                return "Ошибка: Имя класса и код не могут быть пустыми.";

            // Убеждаемся, что имя файла корректное
            string fileName = data.ClassName.EndsWith(".cs") ? data.ClassName : $"{data.ClassName}.cs";
            
            // Путь к папке с инструментами (относительно корня проекта)
            string toolsFolder = "Tools";
            if (!Directory.Exists(toolsFolder))
            {
                return "Ошибка: Папка 'Tools' не найдена.";
            }

            string filePath = Path.Combine(toolsFolder, fileName);

            if (File.Exists(filePath))
                return $"Ошибка: Файл {fileName} уже существует в папке Tools.";

            await File.WriteAllTextAsync(filePath, data.Code);

            return $"Успех! Новый инструмент '{data.ClassName}' создан в папке Tools. Перезапустите программу, чтобы агент смог его использовать.";
        }
        catch (JsonException)
        {
            return "Ошибка формата: ожидался JSON с полями 'className' и 'code'.";
        }
        catch (Exception ex)
        {
            return $"Ошибка при создании инструмента: {ex.Message}";
        }
    }
}

public class CreateToolData
{
    public string ClassName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
