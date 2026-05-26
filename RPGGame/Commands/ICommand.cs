namespace RPGGame.Commands;

using RPGGame.MVC;

public interface ICommand
{
    void Execute(GameModel model, int playerId);
}