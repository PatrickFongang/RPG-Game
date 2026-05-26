namespace RPGGame;

using System;
using System.Threading;
using RPGGame.Builder;
using RPGGame.Director;
using RPGGame.Config;
using RPGGame.Logging;
using RPGGame.MVC;
using RPGGame.MVC.Controllers;
using RPGGame.MVC.Views;
using RPGGame.Network;
using RPGGame.Themes;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        ConfigManager.Instance.LoadConfig("config.json");

        bool isServer = false;
        string ip = "127.0.0.1";
        int port = 5555;

        if (args.Length > 0)
        {
            if (args[0] == "--server")
            {
                isServer = true;
                if (args.Length > 1 && int.TryParse(args[1], out int p))
                {
                    port = p;
                }
            }
            else if (args[0] == "--client")
            {
                if (args.Length > 1)
                {
                    var parts = args[1].Split(':');
                    ip = parts[0];
                    if (parts.Length > 1 && int.TryParse(parts[1], out int p))
                    {
                        port = p;
                    }
                }
            }
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Start as a (S)erver or (C)lient?");
            var key = Console.ReadKey(true).Key;
            isServer = (key == ConsoleKey.S);
            Console.Clear();
        }

        if (isServer)
        {
            RunServer(port);
        }
        else
        {
            RunClient(ip, port);
        }
    }

    static void RunServer(int port)
    {
        CompositeLogger compositeLogger = new CompositeLogger();
        compositeLogger.AddLogger(new MemoryLogger());
        
        FileLogger fileLogger = new FileLogger(
            ConfigManager.Instance.Config.LogDirectory,
            "Server" 
        );
        compositeLogger.AddLogger(fileLogger);
        
        compositeLogger.AddLogger(new ConsoleLogger());

        EventLogger.Instance.SetStrategy(compositeLogger);

        IDungeonTheme[] themes = { new LibraryTheme(), new SciFiTheme(), new WealthTheme() };
        IDungeonTheme selectedTheme = themes[new Random().Next(themes.Length)];

        EventLogger.Instance.Log($"Game server started. Theme: {selectedTheme.GetType().Name}");

        DungeonBuilder builder = new DungeonBuilder(20, 40);
        DungeonDirector director = new DungeonDirector(builder);
        
        director.ConstructThemedDungeon(selectedTheme);
        Dungeon generatedDungeon = builder.GetDungeon();

        GameModel gameModel = new GameModel(generatedDungeon)
        {
            WelcomeMessage = selectedTheme.WelcomeMessage
        };

        TcpGameServer server = new TcpGameServer(gameModel, port);
        
        Console.WriteLine($"=== Server ===");
        Console.WriteLine($"Listening on port: {port}");
        Console.WriteLine($"Loaded Theme: {selectedTheme.GetType().Name}");
        
        server.Start();
        
        while (Console.ReadKey(true).Key != ConsoleKey.Escape) { }
    }

    static void RunClient(string ip, int port)
    {
        Console.WriteLine($"Connecting to the server {ip}:{port}...");
        
        TcpGameClient client = new TcpGameClient(ip, port);
        client.Connect();

        while (client.LocalPlayerId == -1)
        {
            Thread.Sleep(100);
        }

        ConsoleView view = new ConsoleView();
        view.RenderWelcomeScreen(client.WelcomeMessage);

        client.StartRendering();

        InputController controller = new InputController(client.LocalPlayerId, client.SendCommand, client.ShowLogsLocally);

        while (true)
        {
            if (client.IsLocalGameOver)
            {
                client.DisconnectAndShowGameOver(); 
                Console.ReadKey(true);              
                break;                              
            }

            controller.ProcessInput();
            Thread.Sleep(50); 
        }
    }
}