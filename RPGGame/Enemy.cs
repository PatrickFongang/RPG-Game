using RPGGame.Movement;
using RPGGame.Observers;
using RPGGame.Logging;

namespace RPGGame;

public abstract class Enemy : IDeathObserver, ISoundObserver
{
    public string Name { get; }
    public char Symbol { get; }
    public int Health { get; set; }
    public int Attack { get; protected set; }
    public int Armor { get; protected set; }

    public int X { get; set; }
    public int Y { get; set; }

    protected IDeathSubject _speciesGroup;
    private readonly IMovementStrategy _movementStrategy;

    protected Enemy(string name, char symbol, int health, int attack, int armor, IDeathSubject speciesGroup, IMovementStrategy movementStrategy)
    {
        Name = name;
        Symbol = symbol;
        Health = health;
        Attack = attack;
        Armor = armor;
        _speciesGroup = speciesGroup;
        _movementStrategy = movementStrategy;
        _speciesGroup.Attach(this);
    }

    public abstract void OnFellowEnemyDied();

    public void Die()
    {
        _speciesGroup.Detach(this);
        _speciesGroup.NotifyDeath();
    }

    public void PerformMove(Dungeon dungeon)
    {
        _movementStrategy.Move(this, dungeon);
    }

    public void OnSoundHeard(int sourceX, int sourceY, int range, Dungeon dungeon)
    {
        int distance = CalculateSoundDistance(X, Y, sourceX, sourceY, dungeon, range);

        if (distance != -1 && distance <= range)
        {
            EventLogger.Instance.Log($"{Name} at ({X}, {Y}) heard a noise from ({sourceX}, {sourceY}) at a distance of {distance} steps.");
        }
    }

    private int CalculateSoundDistance(int startX, int startY, int targetX, int targetY, Dungeon dungeon, int maxRange)
    {
        if (startX == targetX && startY == targetY) return 0;

        var queue = new Queue<(int x, int y, int dist)>();
        var visited = new bool[dungeon.Rows, dungeon.Columns];
        
        queue.Enqueue((startX, startY, 0));
        visited[startY, startX] = true;

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        while (queue.Count > 0)
        {
            var curr = queue.Dequeue();

            if (curr.x == targetX && curr.y == targetY)
                return curr.dist;

            if (curr.dist >= maxRange) 
                continue; 

            for (int i = 0; i < 4; i++)
            {
                int nx = curr.x + dx[i];
                int ny = curr.y + dy[i];

                if (nx >= 0 && nx < dungeon.Columns && ny >= 0 && ny < dungeon.Rows)
                {
                    if (!visited[ny, nx] && dungeon[ny, nx].IsPassable)
                    {
                        visited[ny, nx] = true;
                        queue.Enqueue((nx, ny, curr.dist + 1));
                    }
                }
            }
        }

        return -1; 
    }
}

public class Goblin : Enemy
{
    public Goblin(IDeathSubject speciesGroup) 
        : base("Goblin", 'g', 30, 8, 2, speciesGroup, new RandomMovementStrategy())
    { }

    public override void OnFellowEnemyDied()
    {
        Attack = Math.Max(1, Attack - 2);
        Armor = Math.Max(0, Armor - 1);
    }
}

public class Skeleton : Enemy
{
    public Skeleton(IDeathSubject speciesGroup) 
        : base("Skeleton", 's', 45, 12, 4, speciesGroup, new RandomMovementStrategy())
    { }

    public override void OnFellowEnemyDied()
    {
        Attack += 2;
        Armor += 1;
    }
}

public class AggressiveBriefcase : Enemy
{
    public AggressiveBriefcase(IDeathSubject speciesGroup) 
        : base("Aggressive Briefcase", 'A', 35, 10, 4, speciesGroup, new RandomMovementStrategy())
    { }

    public override void OnFellowEnemyDied()
    {
        Armor = 0;
        Health = Math.Max(1, Health - 10);
    }
}

public class AnimatedSafe : Enemy
{
    public AnimatedSafe(IDeathSubject speciesGroup) 
        : base("Animated Safe", 'S', 80, 20, 10, speciesGroup, new RandomMovementStrategy())
    { }

    public override void OnFellowEnemyDied()
    {
        Armor += 5;
        Attack += 2;
    }
}

public class CleaningBot : Enemy
{
    public CleaningBot(IDeathSubject speciesGroup) 
        : base("Cleaning Bot", 'c', 30, 8, 2, speciesGroup, new RandomMovementStrategy())
    { }

    public override void OnFellowEnemyDied()
    {
        Health = Math.Max(1, Health / 2);
        Attack = Math.Max(1, Attack - 4);
    }
}

public class SecurityDrone : Enemy
{
    public SecurityDrone(IDeathSubject speciesGroup) 
        : base("Security Drone", 'D', 60, 15, 6, speciesGroup, new RandomMovementStrategy())
    { }

    public override void OnFellowEnemyDied()
    {
        Attack *= 2;
        Health += 10;
    }
}