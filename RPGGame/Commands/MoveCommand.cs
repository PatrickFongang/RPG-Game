namespace RPGGame.Commands;

public class MoveCommand : ICommand
{
    private readonly Player _player;
    private readonly Builder _builder;
    private readonly int _dx, _dy;
    
    public MoveCommand(Player player, Builder builder, int dx, int dy)
    {
        _player = player;
        _builder = builder;
        _dx = dx;
        _dy = dy;
    }

    public void Execute()
    {
        int targetX = _player.X + _dx;
        int targetY = _player.Y + _dy;

        if (targetX < 0 || targetX >= _builder.Columns || targetY < 0 || targetY >= _builder.Rows) return;
        if (!_builder[targetY, targetX].IsPassable) return;

        _player.X = targetX;
        _player.Y = targetY;
    }
}