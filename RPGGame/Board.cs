using RPGGame.Items;

namespace RPGGame;

public class Board
{
    public int Rows { get; }
    public int Columns { get; }
    private readonly Cell[,] _board;

    public Cell this[int rows, int cols] => _board[rows, cols];

    public Board(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;

        _board = new Cell[Rows, Columns];
    }

    public void FillTheBoard(int wallPercentage)
    {
        Random random = new Random();
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (i == 0 && j == 0)
                {
                    _board[i, j] = new GroundCell();
                    continue;
                }
                
                _board[i, j] = random.Next(0, 100) < wallPercentage ? new WallCell() : new GroundCell();

                if (_board[i, j].IsPassable)
                {
                    SpawnItems(i, j);
                }
            }
        }
    }

    private void SpawnItems(int row, int column)
    {
        List<Item> itemList = new List<Item>();
        itemList.Add(new Weapon("Rusty Dagger", '/', 3, damage: 5,
            isTwoHanded: false, "Covered in rust, but still pointy enough to do some damage."));
        itemList.Add(new Weapon("Steel Sword", 't', 2, damage: 12,
            isTwoHanded: false, "A classic, sharp blade forged by a local blacksmith. Reliable in combat."));
        itemList.Add(new Weapon("Orcish Greataxe", 'T', 1, damage: 25,
            isTwoHanded: true, "Heavy and devastating. You will need both hands to swing this monster."));

        itemList.Add(new JunkItem("Old Boot", 'b', 5, "Someone lost it ages ago"));
        itemList.Add(new JunkItem("Skull", 'x', 3, "Empty inside."));
        itemList.Add(new JunkItem("Broken Compas", 'o', 3, "It only points south."));

        itemList.Add(new CoinItem("Coin", 'c', 3,5,
            "A small leather pouch clinking with standard copper coins."));
        itemList.Add(new GoldItem("Gold", 'G', 2,15,
            "A shiny, heavy ingot of solid gold. Merchants will love this."));

        Random random = new Random();
        foreach (Item item in itemList)
        {
            if (random.Next(0, 100) < item.Rarity)
            {
                _board[row, column].ItemsOnGround.Push(item);
            }
        }
    }
}