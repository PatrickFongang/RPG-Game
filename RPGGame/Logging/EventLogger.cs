namespace RPGGame.Logging;

public class EventLogger
{
    private static EventLogger? _instance;
    public static EventLogger Instance => _instance ??= new EventLogger();

    private ILogger? _loggerStrategy;
    
    private EventLogger() {}

    public void SetStrategy(ILogger logger)
    {
        _loggerStrategy = logger;
    }

    public void Log(string message)
    {
        _loggerStrategy?.Log(message);
    }

    public IEnumerable<string> GetLogs()
    {
        return _loggerStrategy?.GetLogs() ?? Array.Empty<string>();
    }
}