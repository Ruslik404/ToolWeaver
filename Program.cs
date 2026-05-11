
using ToolWeaver.Services;
using ToolWeaver.Models; 
using ToolWeaver.Tools;

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
            new CreateFileTool() 
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

            foreach (var tool in tools)
            {
                if (!currentAnswer.Contains(tool.Name, StringComparison.OrdinalIgnoreCase)) 
                    continue;

                // Вырезаем содержимое в ответе:
                string args = ExtractArgs(currentAnswer, tool.Name);

                // Проверка на права:
                if (tool.RequiresConfirmation && !ConfirmExecution(tool.Name, args))
                    continue;

                // ВЫПОЛНЕНИЕ
                string result = await tool.ExecuteAsync(args);

                // ОБНОВЛЯЕМ КОНТЕКСТ
                history.Add(new Message { Role = "assistant", Content = $"{tool.Name}: {args}" });
                history.Add(new Message { Role = "user", Content = $"System result: {result}" });

                currentAnswer = await aiService.AskAI(history);

                toolExecuted = true; // Помечаем, что мы что-то выполнили
                break; 
            }
        } while(toolExecuted);

        return currentAnswer;
    }

    // Вспомогательная функция для выреза
    static string ExtractArgs(string text, string toolName)
    {
        // Ищем, где начинается название инструмента
        int nameIndex = text.IndexOf(toolName, StringComparison.OrdinalIgnoreCase);
        if (nameIndex == -1) return string.Empty;

        // Ищем двоеточие ТОЛЬКО после названия инструмента
        int start = text.IndexOf(":", nameIndex);
        // Ищем закрывающую скобку ТОЛЬКО после этого двоеточия
        int end = text.IndexOf("]", nameIndex);

        if (start != -1 && end > start)
        {
            // Вырезаем только то, что внутри скобок ЭТОГО инструмента
            return text.Substring(start + 1, end - start - 1).Trim();
        }

        return string.Empty;
    }


    // Вспомогательнвая функция для подтверждения
    static bool ConfirmExecution(string toolName, string args)
    {
        Console.WriteLine($"\n[SYSTEM] Запрос: {toolName} ({args})");
        Console.Write("[SYSTEM] Разрешить? (y/n): ");
        return Console.ReadLine()?.ToLower() == "y";
    }
}

