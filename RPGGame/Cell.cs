using RPGGame.Items;
namespace RPGGame;

public enum TerrainType
{
    Ground,
    Wall
}
public class Cell
{
    public TerrainType Terrain;
    public Stack<Item> ItemsOnGround;

    public Cell(TerrainType terrain)
    {
        this.Terrain = terrain;
        this.ItemsOnGround = new Stack<Item>();
    }
}