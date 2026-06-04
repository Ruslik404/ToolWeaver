namespace ToolWeaver.Tools;

public class GetTimeTool : ITool
{
    public string Name => "GET_TIME"; 
    public string Description => "Узнать текущее системное время. Формат: {\"tool\": \"GET_TIME\", \"args\": {}}";
    public bool RequiresConfirmation => false; 

    public async Task<string> ExecuteAsync(string args)
    {
        return $"Текущее время: {DateTime.Now:HH:mm}.";
    }
}
