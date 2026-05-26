namespace RPGGame.Commands;

using System;
using RPGGame.MVC;

public class ActionCommand : ICommand
{
    private readonly Action<GameModel, int> _action;

    public ActionCommand(Action<GameModel, int> action)
    {
        _action = action;
    }

    public void Execute(GameModel model, int playerId)
    {
        _action(model, playerId);
    }
}