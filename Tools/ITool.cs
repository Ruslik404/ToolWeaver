namespace ToolWeaver.Tools; 
public interface ITool
{
    string Name { get; }
    string Description { get; }
    object Parameters { get; } 
    bool RequiresConfirmation { get; } 
    Task<string> ExecuteAsync(string args); 
}