using RPGGame.Logging;

namespace RPGGame.Commands;

public class MoveCommand : ICommand
{
    private readonly Player _player;
    private readonly Dungeon _dungeon;
    private readonly int _dx, _dy;
    private readonly GameEngine _gameEngine;
    
    public MoveCommand(Player player, Dungeon dungeon, GameEngine gameEngine, int dx, int dy)
    {
        _player = player;
        _dungeon = dungeon;
        _gameEngine = gameEngine;
        _dx = dx;
        _dy = dy;
    }

    public void Execute()
    {
        int targetX = _player.X + _dx;
        int targetY = _player.Y + _dy;

        if (targetX < 0 || targetX >= _dungeon.Columns || targetY < 0 || targetY >= _dungeon.Rows) return;
    
        Cell targetCell = _dungeon[targetY, targetX];

        if (targetCell.Enemy != null)
        {
            _gameEngine.ResolveCombat(targetCell.Enemy, targetCell); 
            return; 
        }

        if (!targetCell.IsPassable)
        {
            EventLogger.Instance.Log($"{_player.PlayerName} tried to walk into a wall.");
            return;
        }

        _player.X = targetX;
        _player.Y = targetY;
    }
}