namespace RPGGame.Movement;

public class RandomMovementStrategy : IMovementStrategy
{
    private static readonly Random _random = new Random();

    public void Move(Enemy enemy, Dungeon dungeon)
    {
        var possibleMoves = new List<(int x, int y)>();
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int nx = enemy.X + dx[i];
            int ny = enemy.Y + dy[i];

            if (nx >= 0 && nx < dungeon.Columns && ny >= 0 && ny < dungeon.Rows)
            {
                var cell = dungeon[ny, nx];
                if (cell.IsPassable && cell.Enemy == null)
                {
                    possibleMoves.Add((nx, ny));
                }
            }
        }

        if (possibleMoves.Count > 0)
        {
            var move = possibleMoves[_random.Next(possibleMoves.Count)];
            
            dungeon[enemy.Y, enemy.X].Enemy = null;
            
            enemy.X = move.x;
            enemy.Y = move.y;
            
            dungeon[enemy.Y, enemy.X].Enemy = enemy;
        }
    }
}