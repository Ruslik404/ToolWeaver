namespace ToolWeaver.Services;

public static class PromptService
{
    public static string GetSystemPrompt()
    {
        //Получаем путь к папке, где лежит сама программа
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        string path = Path.Combine(baseDir, "Prompts", "default.md");

        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);
            //Console.WriteLine($"[DEBUG]: Промпт загружен из {path}"); 
            return content;
        }

        Console.WriteLine($"[ERROR]: Файл промпта НЕ НАЙДЕН по пути: {path}");
        return "Ты полезный ИИ-помощник."; 
    }

}
