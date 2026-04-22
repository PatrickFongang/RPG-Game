namespace RPGGame.Logging;

public class CompositeLogger : ILogger
{
    private readonly List<ILogger> _loggers = new();

    public void AddLogger(ILogger logger)
    {
        _loggers.Add(logger);
    }

    public void Log(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.Log(message);
        }
    }

    public IEnumerable<string> GetLogs()
    {
        var loggerWithMostLogs = _loggers
            .OrderByDescending(l => l.GetLogs().Count())
            .FirstOrDefault();

        return loggerWithMostLogs?.GetLogs() ?? Array.Empty<string>();
    }
}