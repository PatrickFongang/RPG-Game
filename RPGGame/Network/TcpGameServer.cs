namespace RPGGame.Network;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using RPGGame.MVC;
using RPGGame.Network.DTOs;
using RPGGame.Commands;
using RPGGame.Logging;

public class TcpGameServer
{
    private readonly GameModel _model;
    private readonly int _port;
    private TcpListener _listener;
    private int _nextPlayerId = 1;
    private readonly Dictionary<string, Func<ICommand>> _commandFactory;

    public TcpGameServer(GameModel model, int port)
    {
        _model = model;
        _port = port;
        
        _commandFactory = new Dictionary<string, Func<ICommand>>
        {
            { "MOVE_UP", () => new MoveCommand(0, -1) },
            { "MOVE_DOWN", () => new MoveCommand(0, 1) },
            { "MOVE_LEFT", () => new MoveCommand(-1, 0) },
            { "MOVE_RIGHT", () => new MoveCommand(1, 0) },
            { "PICKUP", () => new ActionCommand(CommandActions.PickUp) },
            { "EQUIP", () => new ActionCommand(CommandActions.Equip) },
            { "DROP", () => new ActionCommand(CommandActions.Drop) },
            {"FREE_LEFT", () => new ActionCommand(CommandActions.FreeLeftHand)},
            {"FREE_RIGHT", () => new ActionCommand(CommandActions.FreeRightHand)},
            {"CYCLE_STRATEGY", () => new ActionCommand(CommandActions.CycleAttackStrategy)},
            {"INV_UP", () => new ActionCommand(CommandActions.MoveInventoryUp)},
            {"INV_DOWN", () => new ActionCommand(CommandActions.MoveInventoryDown)}
        };
    }

    public void Start()
    {
        _listener = new TcpListener(IPAddress.Any, _port);
        _listener.Start();
        
        Task.Run(AcceptClientsAsync);
        Task.Run(EnemyTickLoopAsync);
    }

    private async Task EnemyTickLoopAsync()
    {
        while (true)
        {
            await Task.Delay(1500); 
            _model.TickEnemies();
        }
    }

    private async Task AcceptClientsAsync()
    {
        while (true)
        {
            if (_nextPlayerId > 9) break;

            var client = await _listener.AcceptTcpClientAsync();
            int currentId = _nextPlayerId++;
            
            var player = new Player(_model.Dungeon.Columns / 2, _model.Dungeon.Rows / 2);
            
            foreach (var enemy in _model.Dungeon.ActiveEnemies)
            {
                player.AttachSoundObserver(enemy);
            }

            _model.AddPlayer(currentId, player);

            Task.Run(() => HandleClientAsync(client, currentId));
        }
    }

   private async Task HandleClientAsync(TcpClient client, int playerId)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream);
        
        var initDto = new ClientInitDTO 
        { 
            AssignedPlayerId = playerId,
            WelcomeMessage = _model.WelcomeMessage
        };
        
        await writer.WriteLineAsync(Serializer.Serialize(initDto));
        await writer.FlushAsync();

        string joinJson = await reader.ReadLineAsync();
        if (joinJson != null)
        {
            var joinDto = Serializer.Deserialize<ClientJoinDTO>(joinJson);
            
            lock (_model.StateLock) 
            {
                if (_model.Players.TryGetValue(playerId, out var player))
                {
                    player.PlayerName = joinDto.PlayerName; 
                    
                    EventLogger.Instance.Log($"Game started for player: {player.PlayerName}");
                }
            }
        }

        var remoteView = new RemoteClientView(writer, playerId);
        _model.AttachView(remoteView);

        _model.NotifyViews();

        try
        {
            while (client.Connected)
            {
                string json = await reader.ReadLineAsync();
                if (json == null) break;

                var commandDto = Serializer.Deserialize<CommandDTO>(json);
                if (commandDto != null && commandDto.PlayerId == playerId)
                {
                    if (_commandFactory.TryGetValue(commandDto.CommandType, out var factoryMethod))
                    {
                        var command = factoryMethod();
                        command.Execute(_model, playerId);
                    }
                }
            }
        }
        finally
        {
            _model.DetachView(remoteView);
            _model.RemovePlayer(playerId);
            client.Close();
        }
    }
}