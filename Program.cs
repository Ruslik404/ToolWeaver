using System.Text.RegularExpressions; 
using ToolWeaver.Services;
using ToolWeaver.Models; 
using ToolWeaver.Tools;
using System.Text.Json;
using System.Reflection;

class Program
{
    static async Task Main(string[] args)
    {
        using var aiService = new AIService();

        // Автоматическая загрузка всех инструментов, реализующих ITool
        var tools = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(ITool).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => (ITool)Activator.CreateInstance(t)!)
            .ToList();

        Console.WriteLine($"ToolWeaver started with {tools.Count} tools.");

        // Генерируем JSON-описание инструментов для промпта
        var toolsForAi = tools.Select(t => new {
            name = t.Name,
            description = t.Description,
            parameters = t.Parameters
        });
        string toolsJson = JsonSerializer.Serialize(toolsForAi, new JsonSerializerOptions { WriteIndented = true });

        // Формируем финальный промпт
        string basePrompt = PromptService.GetSystemPrompt();
        string fullPrompt = basePrompt.Replace("(Список инструментов генерируется автоматически)", toolsJson);

        var chatHistory = new List<Message>(); 
        chatHistory.Add(new Message { Role = "system", Content = fullPrompt });

        var notifyTool = tools.FirstOrDefault(t => t.Name == "DESKTOP_NOTIFY");
        var cts = new CancellationTokenSource();

        // Запускаем "фоновый мозг" (не блокирует консоль)
        _ = RunProactiveAgent(aiService, notifyTool, cts.Token);

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


    
    static async Task RunProactiveAgent(AIService aiService, ITool? notifyTool, CancellationToken ct)
    {
        int intervalMinutes = 3; // тест
    
        //Console.WriteLine("[Proactive] Фоновая задача запущена.");
    
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), ct);
            //Console.WriteLine($"[Proactive] Тик таймера. Пытаюсь спросить ИИ...");
    
            try
            {
                if (notifyTool == null) 
                {
                    //Console.WriteLine("[Proactive] ОШИБКА: notifyTool не найден!");
                    continue;
                }
    
                // ← ИСПРАВЛЕНО: добавлен "user" для совместимости с Z.AI
                var prompt = new List<Message>
                {
                    new Message { Role = "system", Content = "Ты — дружелюбный помощник. Спроси одним коротким предложением, нужна ли помощь. Звучи естественно." },
                    new Message { Role = "user", Content = "..." }
                };
    
                string reply = await aiService.AskAI(prompt);
                Console.WriteLine($"[Proactive] ИИ ответил: {reply}");
    
                string safeMsg = reply.Replace("\"", "'");
                await notifyTool.ExecuteAsync($"{{\"title\": \"ToolWeaver\", \"message\": \"{safeMsg}\"}}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Proactive] ОШИБКА: {ex.Message}");
            }
        }
    }



}