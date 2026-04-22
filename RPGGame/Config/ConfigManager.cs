using System.Text.Json;

namespace RPGGame.Config;

public class ConfigManager
{
    private static ConfigManager? _instance;
    public static ConfigManager Instance => _instance ??= new ConfigManager();
    
    public GameConfig Config { get; private set; } = new GameConfig();
    
    private ConfigManager() {}

    public void LoadConfig(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Config = JsonSerializer.Deserialize<GameConfig>(json) ?? new GameConfig();
        }
        else
        {
            string defaultJson = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, defaultJson);
        }
    }
}