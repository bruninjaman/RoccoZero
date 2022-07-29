using Divine.Game;

namespace Farmling.LoggerService;

public static class Logger
{
    static Logger()
    {
        IsEnabled = false;
    }

    public static bool IsEnabled { get; set; }

    public static void Log(string message)
    {
        if (IsEnabled)
            Console.WriteLine($"[{GameManager.RawGameTime}]: {message}");
    }
    public static void LogError(Exception ex, string message)
    {
        Console.WriteLine($"[{GameManager.RawGameTime}]: Exception: Message: {message}");
    }
}
