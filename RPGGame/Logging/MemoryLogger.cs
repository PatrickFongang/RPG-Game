namespace RPGGame.Logging;

public class MemoryLogger : ILogger
{
    private readonly List<string> _logs = new();

    public void Log(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        _logs.Add($"[{timestamp}]: {message}");
    }

    public IEnumerable<string> GetLogs()
    {
        return _logs.AsReadOnly();
    }
}