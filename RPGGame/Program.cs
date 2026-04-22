using System.Globalization;
using RPGGame;
using RPGGame.Builder;
using RPGGame.Director;
using RPGGame.Logging;
using RPGGame.Config;
using RPGGame.Themes;

Console.OutputEncoding = System.Text.Encoding.UTF8;

ConfigManager.Instance.LoadConfig("config.json");

CompositeLogger compositeLogger = new CompositeLogger();
compositeLogger.AddLogger(new MemoryLogger());

FileLogger fileLogger = new FileLogger(
    ConfigManager.Instance.Config.LogDirectory, 
    ConfigManager.Instance.Config.PlayerName
);
compositeLogger.AddLogger(fileLogger);

EventLogger.Instance.SetStrategy(compositeLogger);

IDungeonTheme[] themes = { new LibraryTheme(), new SciFiTheme(), new WealthTheme() };
IDungeonTheme selectedTheme = themes[new Random().Next(themes.Length)];

Console.Clear();
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=== WELCOME TO THE DUNGEON ===");
Console.ResetColor();
Console.WriteLine();

Console.WriteLine(selectedTheme.WelcomeMessage);

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("Press any key to begin your adventure...");
Console.ResetColor();
Console.ReadKey(true);
Console.Clear();

EventLogger.Instance.Log($"Game started for player: {ConfigManager.Instance.Config.PlayerName}");

DungeonBuilder builder = new DungeonBuilder(20, 40);
DungeonDirector director = new DungeonDirector(builder);

director.ConstructThemedDungeon(selectedTheme);
Dungeon generatedDungeon = builder.GetDungeon();

GameEngine newGameEngine = new GameEngine(generatedDungeon);
newGameEngine.StartGame();