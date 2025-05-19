using SampleBankOperations.Core.Interfaces;

namespace SampleBankOperations.Infrastructure.Logging;

public class Logger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[LOG] {DateTime.Now}: {message}");
    }
}
