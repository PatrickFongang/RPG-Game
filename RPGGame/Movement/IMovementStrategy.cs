namespace RPGGame.Movement;

public interface IMovementStrategy
{
    void Move(Enemy enemy, Dungeon dungeon);
}