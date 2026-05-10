using ToolWeaver.Services;

namespace ToolWeaver;

class Program
{
    static async Task Main(string[] args)
    {
        using var aiService = new AIService();

        Console.WriteLine("ToolWeaver started");

        while(true) {
            string question = Console.ReadLine() ?? string.Empty; 
            if(question == "exit") break;

            string answer = await aiService.AskAI(question);

            Console.WriteLine($"ai: {answer}");

        }

    }
}

