using System.Text.RegularExpressions; 
using ToolWeaver.Services;
using ToolWeaver.Models; 
using ToolWeaver.Tools;
using System.Text.Json;  // ← для JsonDocument
class Program
{
    static async Task Main(string[] args)
    {
        using var aiService = new AIService();


        var chatHistory = new List<Message>(); // creat History chat for ai

        //PROMPT
        string prompt = PromptService.GetSystemPrompt();
        chatHistory.Add(new Message { Role = "system", Content = prompt });

        //все навыки
        var tools = new List<ITool> {
            new GetTimeTool(),
            new CreateFileTool(),
            new WriteFileTool(),
            new ListFilesTool(),
            new ReadFileTool(),
            new OpenAppTool()
        };

        Console.WriteLine("ToolWeaver started");


        while(true) 
        {   
            Console.Write("you: ");
            string question = Console.ReadLine() ?? string.Empty; 
            if(question == "exit") break;

            chatHistory.Add(new Message { Role = "user", Content = question });

            // Получаем первичный ответ
            string answer = await aiService.AskAI(chatHistory);

            // Обрабатываем инструменты 
            answer = await ProcessAiTools(answer, tools, chatHistory, aiService);

            // Финальный вывод
            chatHistory.Add(new Message { Role = "assistant", Content = answer });
            Console.WriteLine($"ai: {answer}");

        }

    }

    static async Task<string> ProcessAiTools(string aiAnswer, List<ITool> tools, List<Message> history, AIService aiService)
    {
        string currentAnswer = aiAnswer;
        bool toolExecuted;

        do
        {
            toolExecuted = false; //ниче не сделали

            // 1. Парсим ответ один раз
            var parsed = TryParseToolCall(currentAnswer);
            if (!parsed.Success) break; // Нет валидного JSON → выходим из цикла
            
            // 2. Ищем инструмент по имени из JSON
            var tool = tools.FirstOrDefault(t => t.Name.Equals(parsed.ToolName, StringComparison.OrdinalIgnoreCase));
            if (tool == null) break; // Инструмент не зарегистрирован → выходим
            
            string args = parsed.ArgsJson ?? string.Empty;

            // Проверка на права:
            if (tool.RequiresConfirmation && !ConfirmExecution(tool.Name, args))
            {
                string denialResult = "Ошибка: Пользователь отклонил запрос на выполнение этой команды.";
                history.Add(new Message { Role = "assistant", Content = $"{tool.Name}: {args}" });
                history.Add(new Message { Role = "user", Content = $"System result: {denialResult}" });
                currentAnswer = await aiService.AskAI(history);
                toolExecuted = true; 
                continue; 
            }
            // ВЫПОЛНЕНИЕ
            string result = await tool.ExecuteAsync(args);
            // ОБНОВЛЯЕМ КОНТЕКСТ
            history.Add(new Message { Role = "assistant", Content = $"{tool.Name}: {args}" });
            history.Add(new Message { Role = "user", Content = $"System result: {result}" });
            currentAnswer = await aiService.AskAI(history);
            toolExecuted = true; // Помечаем, что мы что-то выполнили
            continue; 
            
        } while(toolExecuted);

        return currentAnswer;
    }

    // Вспомогательная функция для выреза

    public class ParsedToolCall
    {
        public string? ToolName { get; set; }
        public string? ArgsJson { get; set; }
        public bool Success { get; set; }
    }
    static ParsedToolCall TryParseToolCall(string text)
    {
        try
        {
            // Ищем первый { и последний } — вырезаем потенциальный JSON
            int start = text.IndexOf('{');
            int end = text.LastIndexOf('}');

            if (start == -1 || end == -1 || end <= start)
                return new ParsedToolCall { Success = false };

            string json = text.Substring(start, end - start + 1);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("tool", out var toolProp) && 
                root.TryGetProperty("args", out var argsProp))
            {
                return new ParsedToolCall 
                { 
                    ToolName = toolProp.GetString(), 
                    ArgsJson = argsProp.GetRawText(), 
                    Success = true 
                };
            }
        }
        catch { /* невалидный JSON — игнорируем */ }

        return new ParsedToolCall { Success = false };
    }


    // Вспомогательнвая функция для подтверждения
    static bool ConfirmExecution(string toolName, string args)
    {
        Console.WriteLine($"\n[SYSTEM] Запрос: {toolName} ({args})");
        Console.Write("[SYSTEM] Разрешить? (y/n): ");
        return Console.ReadLine()?.ToLower() == "y";
    }
}

