using RPGGame.Items;
namespace RPGGame;

public abstract class Cell
{
    public Stack<Item> ItemsOnGround { get; } = new Stack<Item>();
    public abstract bool IsPassable { get; } 
    public abstract char GetSymbol();
    public abstract ConsoleColor GetColor();
}

public class GroundCell : Cell
{
    public override bool IsPassable => true;
    public override char GetSymbol() => '▓'; 
    public override ConsoleColor GetColor() => ConsoleColor.DarkGreen;
}

public class WallCell : Cell
{
    public override bool IsPassable => false;
    public override char GetSymbol() => '█';
    public override ConsoleColor GetColor() => ConsoleColor.DarkGray;
}