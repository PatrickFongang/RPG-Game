namespace RPGGame.Network;

using System.IO;
using RPGGame.MVC;

public class RemoteClientView : IView
{
    private readonly StreamWriter _writer;
    private readonly int _playerId;

    public RemoteClientView(StreamWriter writer, int playerId)
    {
        _writer = writer;
        _playerId = playerId;
    }

    public void Render(GameModel model, int localPlayerId)
    {
        var state = GameStateSnapshot.CreateSnapshot(model);
        string json = Serializer.Serialize(state);
        _writer.WriteLine(json);
        _writer.Flush();
    }

    public void DisplayMessage(string message)
    {
    }
}