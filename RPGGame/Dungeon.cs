using RPGGame.Items;

namespace RPGGame;

public class Dungeon
{
    public int Rows { get; }
    public int Columns { get; }
    private readonly Cell[,] _board;

    public bool HasPickups { get; set; }
    public bool HasInventoryItems { get; set; }

    public Cell this[int rows, int cols]
    {
        get => _board[rows, cols];
        set => _board[rows, cols] = value;
    }

    public Dungeon(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        _board = new Cell[Rows, Columns];
    }
}