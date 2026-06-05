using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

namespace ToolWeaver.Tools;

public class CreateDirectoryTool : ITool 
{
    public string Name => "CREATE_DIRECTORY";
    public string Description => "Создает папку по указанному пути";
    public object Parameters => new { path = "string" };
    public bool RequiresConfirmation => false;

    public async Task<string> ExecuteAsync(string args) 
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var request = JsonSerializer.Deserialize<CreateDirectoryRequest>(args, options);
        
        if (request == null || string.IsNullOrEmpty(request.path))
        {
            return "Ошибка: путь не указан";
        }
        
        try
        {
            string fullPath = Path.Combine("Workspace", request.path);
            Directory.CreateDirectory(fullPath);
            return $"Папка успешно создана: {request.path} (внутри Workspace)";
        }
        catch (Exception ex)
        {
            return $"Ошибка при создании папки: {ex.Message}";
        }
    }
    
    private class CreateDirectoryRequest
    {
        public string path { get; set; } = string.Empty;
    }
};