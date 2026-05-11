namespace ToolWeaver.Tools; 
public interface ITool
{
    string Name { get; }
    bool RequiresConfirmation { get; } 

    // Метод Execute должен возвращать строку (результат работы), 
    // которую мы потом подсунем ИИ.
    // Параметр args — это то, что мы вытащим из ответа ИИ.
    Task<string> ExecuteAsync(string args); 
}