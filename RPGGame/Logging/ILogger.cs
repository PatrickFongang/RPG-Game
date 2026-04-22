namespace RPGGame.Logging;

public interface ILogger
{
    void Log(string message);
    IEnumerable<string> GetLogs();
}