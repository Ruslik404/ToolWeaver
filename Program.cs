using ToolWeaver.Services;
using ToolWeaver.Models; 

namespace ToolWeaver;

class Program
{
    static async Task Main(string[] args)
    {
        using var aiService = new AIService();


        var chatHistory = new List<Message>(); // creat History chat for ai

        //PROMPT
        string prompt = PromptService.GetSystemPrompt();
        chatHistory.Add(new Message { Role = "system", Content = prompt });

        Console.WriteLine("ToolWeaver started");

        while(true) {
            string question = Console.ReadLine() ?? string.Empty; 
            if(question == "exit") break;

            chatHistory.Add(new Message { Role = "user", Content = question }); // add user`s text in the history chat

            string answer = await aiService.AskAI(chatHistory);

            if (answer.Contains("[GET_TIME]"))
            {
                string currentTime = DateTime.Now.ToString("HH:mm");

                // Добавляем в историю факт запроса от ИИ и ответ системы
                chatHistory.Add(new Message { Role = "assistant", Content = "[GET_TIME]" });
                chatHistory.Add(new Message { Role = "user", Content = $"Системная информация: Текущее время {currentTime}. Ответь пользователю." });

                // Повторный запрос, чтобы ИИ «переварил» время и ответил красиво
                answer = await aiService.AskAI(chatHistory);
            }

            chatHistory.Add(new Message { Role = "assistant", Content = answer }); // and ai`s text im the histoty
            Console.WriteLine($"ai: {answer}");

        }

    }
}

