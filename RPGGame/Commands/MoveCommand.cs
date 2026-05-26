namespace RPGGame.Commands;

using System.Linq;
using RPGGame.MVC;
using RPGGame.Logging;

public class MoveCommand : ICommand
{
    private readonly int _dx;
    private readonly int _dy;

    public MoveCommand(int dx, int dy)
    {
        _dx = dx;
        _dy = dy;
    }

    public void Execute(GameModel model, int playerId)
    {
        if (!model.Players.TryGetValue(playerId, out Player player)) return;

        int targetX = player.X + _dx;
        int targetY = player.Y + _dy;

        if (targetX < 0 || targetX >= model.Dungeon.Columns || targetY < 0 || targetY >= model.Dungeon.Rows) return;

        Cell targetCell = model.Dungeon[targetY, targetX];

        if (targetCell.Enemy != null)
        {
            model.ResolveCombat(player, targetCell.Enemy, targetCell);
            return;
        }
        
        var otherPlayer = model.Players.Values.FirstOrDefault(p => p.X == targetX && p.Y == targetY);
        if (otherPlayer != null)
        {
            return; 
        }

        if (!targetCell.IsPassable)
        {
            EventLogger.Instance.Log($"{player.PlayerName} tried to walk into a wall.");
            model.NotifyViews();
            return;
        }

        player.X = targetX;
        player.Y = targetY;
        
        model.NotifyViews();
    }
}