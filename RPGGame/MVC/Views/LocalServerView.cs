namespace RPGGame.MVC.Views;

using System;
using RPGGame.Network;

public class LocalServerView : IView
{
    private readonly ConsoleView _consoleView;
    private readonly int _localPlayerId;

    public LocalServerView(int localPlayerId)
    {
        _consoleView = new ConsoleView();
        _localPlayerId = localPlayerId;
    }

    public void Render(GameModel model, int localPlayerId)
    {
        var state = GameStateSnapshot.CreateSnapshot(model);
        _consoleView.Render(state, _localPlayerId);
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }
}