using Application.Interfaces.Services;

namespace Infrastructure.Services;

public sealed class ConsoleLoggerServis : ILoggerServis
{
    public bool Loguj(string poruka)
    {
        Console.WriteLine($"[LOG {DateTime.Now:yyyy-MM-dd HH:mm:ss}] {poruka}");
        return true;
    }
}