namespace RPGGame.Logging;

public class FileLogger : ILogger
{
    private readonly string _filePath;
    public string FilePath => _filePath;

    public FileLogger(string directoryPath, string playerName)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"{playerName}_{timestamp}.txt";
        
        Directory.CreateDirectory(directoryPath);
        _filePath = Path.Combine(directoryPath, fileName);
    }
    public void Log(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        File.AppendAllText(_filePath, $"[{timestamp}]: {message}{Environment.NewLine}");
    }

    public IEnumerable<string> GetLogs()
    {
        if (File.Exists(_filePath))
        {
            return File.ReadAllLines(_filePath);
        }
        return Array.Empty<string>();
    }
}