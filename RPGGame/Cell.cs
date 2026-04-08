using RPGGame.Items;
namespace RPGGame;

public abstract class Cell
{
    public Enemy? Enemy { get; set; }
    public Stack<Item> ItemsOnGround { get; } = new Stack<Item>();
    public abstract bool IsPassable { get; }

    public virtual char GetSymbol()
    {
        if (Enemy != null) return Enemy.Symbol;
        return GetBaseSymbol();
    }
    public virtual ConsoleColor GetColor()
    {
        if (Enemy != null) return ConsoleColor.Red;
        return GetBaseColor();
    }

    protected abstract char GetBaseSymbol();
    protected abstract ConsoleColor GetBaseColor();
}

public class GroundCell : Cell
{
    public override bool IsPassable => true;
    protected override char GetBaseSymbol() => '▓'; 
    protected override ConsoleColor GetBaseColor() => ConsoleColor.DarkGreen;
}

public class WallCell : Cell
{
    public override bool IsPassable => false;
    protected override char GetBaseSymbol() => '█';
    protected override ConsoleColor GetBaseColor() => ConsoleColor.DarkGray;
}