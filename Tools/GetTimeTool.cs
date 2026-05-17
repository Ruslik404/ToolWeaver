namespace ToolWeaver.Tools;

public class GetTimeTool : ITool
{
    public string Name => "GET_TIME"; 
    public bool RequiresConfirmation => false; 

    public async Task<string> ExecuteAsync(string args)
    {
        return $"Текущее время: {DateTime.Now:HH:mm}.";
    }
}
