namespace RPGGame.Network;

using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using RPGGame.Network.DTOs;
using RPGGame.MVC.Views;
using RPGGame.Config;

public class TcpGameClient
{
    private readonly string _ip;
    private readonly int _port;
    private TcpClient _client;
    private StreamReader _reader;
    private StreamWriter _writer;
    private readonly ConsoleView _view;
    private GameStateDTO _latestState;
    
    public bool IsReadyToRender { get; private set; } = false;
    public bool IsLocalGameOver { get; private set; } = false;
    public bool IsViewingLogs { get; private set; } = false;
    
    public int LocalPlayerId { get; private set; } = -1;
    public string WelcomeMessage { get; private set; }

    public TcpGameClient(string ip, int port)
    {
        _ip = ip;
        _port = port;
        _view = new ConsoleView();
    }

    public void Connect()
    {
        _client = new TcpClient(_ip, _port);
        var stream = _client.GetStream();
        _reader = new StreamReader(stream);
        _writer = new StreamWriter(stream);

        string initJson = _reader.ReadLine();
        if (initJson != null)
        {
            var initDto = Serializer.Deserialize<ClientInitDTO>(initJson);
            LocalPlayerId = initDto.AssignedPlayerId;
            WelcomeMessage = initDto.WelcomeMessage;
        }

        var joinDto = new ClientJoinDTO 
        { 
            PlayerName = ConfigManager.Instance.Config.PlayerName 
        };
        _writer.WriteLine(Serializer.Serialize(joinDto));
        _writer.Flush();

        Task.Run(ReceiveUpdatesAsync);
    }

    public void SendCommand(CommandDTO command)
    {
        command.PlayerId = LocalPlayerId;
        string json = Serializer.Serialize(command);
        _writer.WriteLine(json);
        _writer.Flush();
    }

    private async Task ReceiveUpdatesAsync()
    {
        try
        {
            while (_client.Connected)
            {
                string json = await _reader.ReadLineAsync();
                if (json == null) break;

                var state = Serializer.Deserialize<GameStateDTO>(json);
                if (state != null)
                {
                    _latestState = state;

                    if (state.IsGameOver || (state.Players.ContainsKey(LocalPlayerId) &&
                                             state.Players[LocalPlayerId].Health <= 0))
                    {
                        IsLocalGameOver = true;
                        break;
                    }
                    else if (IsReadyToRender && !IsViewingLogs)
                    {
                        _view.Render(state, LocalPlayerId);
                    }
                }
            }
        }
        catch
        {
        }
    }

    public void DisconnectAndShowGameOver()
    {
        if (_latestState != null)
        {
            _view.RenderGameOverScreen(_latestState, LocalPlayerId);
        }
        _client.Close();
    }

    public void StartRendering()
    {
        IsReadyToRender = true;
        if (_latestState != null)
        {
            _view.Render(_latestState, LocalPlayerId);
        }
    }

    public void ShowLogsLocally()
    {
        if (_latestState != null)
        {
            IsViewingLogs = true;
            
            _view.RenderFullLogsScreen(_latestState);
            
            IsViewingLogs = false;
            
            _view.Render(_latestState, LocalPlayerId);
        }
    }
}