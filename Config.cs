namespace ToolWeaver;

public static class Config
{
    // Вместо прямой строки читаем из переменной окружения или файла
    public static string API_KEY => GetApiKey(); 
    
      public const string MODEL = "openrouter/free";
    public const string API_URL = "https://openrouter.ai/api/v1/chat/completions";

  


    private static string GetApiKey()
    {
        // 1. Сначала проверяем, есть ли такой файл
        if (File.Exists(".env"))
        {
            // Читаем строку, убираем название переменной и пробелы
            var content = File.ReadAllText(".env");
            if (content.Contains("=")) 
                return content.Split('=')[1].Trim();
        }

        // 2. Если файла нет, попробуем взять из системы (на будущее)
        return Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ?? string.Empty;
    }
}
