namespace RPGGame.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
    }

    public IEnumerable<string> GetLogs()
    {
        return Enumerable.Empty<string>();
    }
}