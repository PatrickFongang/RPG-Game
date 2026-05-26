namespace RPGGame.MVC;

public interface IView
{
    void Render(GameModel model, int localPlayerId);
    void DisplayMessage(string message);
}