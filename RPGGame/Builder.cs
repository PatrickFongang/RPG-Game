using System.Globalization;
using RPGGame.Items;

namespace RPGGame;

public class Builder
{
    public int Rows { get; }
    public int Columns { get; }
    private readonly Cell[,] _board;

    private int _numberOfWalls;

    private readonly int _numberOfDifferentJunkItems = 3;
    private readonly int _numberOfDifferentWeapons = 3;
    private readonly int _numberOfDifferentCurrencies = 2;

    public bool HasPickups { get; private set; }
    public bool HasInventoryItems { get; private set; }

    public Cell this[int rows, int cols] => _board[rows, cols];

    public Builder(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;

        _board = new Cell[Rows, Columns];
    }

    public void BuildEmptyDungeon()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                _board[i, j] = new GroundCell();
            }
        }
    }

    public void BuildFilledDungeon()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                _board[i, j] = new WallCell();
            }
        }

        _numberOfWalls = Rows * Columns;
    }

    public void AddRooms(int numberOfRooms)
    {
        if (numberOfRooms < 1 || numberOfRooms > _numberOfWalls) return;

        Random random = new Random();
        for (int i = 0; i < numberOfRooms; i++)
        {
            int row, column;
            do
            {
                row = random.Next(0, Rows);
                column = random.Next(0, Columns);
            } while (_board[row, column].IsPassable);

            _board[row, column] = new GroundCell();
            _numberOfWalls--;
        }
    }

    public void AddJunkItems(int numberOfJunkItems)
    {
        if (numberOfJunkItems < 1) return;
        HasPickups = true;
        HasInventoryItems = true;
        Random random = new Random();

        for (int i = 0; i < numberOfJunkItems; i++)
        {
            (int row, int col) field = GetRandomField(random);

            int index = random.Next(0, _numberOfDifferentJunkItems);
            _board[field.row, field.col].ItemsOnGround.Push(GetJunkItem(index));
        }
    }

    public void AddWeapons(int numberOfWeapons)
    {
        if (numberOfWeapons < 1) return;
        HasPickups = true;
        HasInventoryItems = true;

        Random random = new Random();
        for (int i = 0; i < numberOfWeapons; i++)
        {
            (int row, int col) field = GetRandomField(random);

            int index = random.Next(0, _numberOfDifferentWeapons);
            _board[field.row, field.col].ItemsOnGround.Push(GetWeaponItem(index));
        }
    }

    public void AddCurrencies(int numberOfCurrencies)
    {
        if (numberOfCurrencies < 1) return;
        HasPickups = true;
        Random random = new Random();

        for (int i = 0; i < numberOfCurrencies; i++)
        {
            (int row, int col) field = GetRandomField(random);

            int index = random.Next(0, _numberOfDifferentCurrencies);
            _board[field.row, field.col].ItemsOnGround.Push(GetCurrencyItem(index));
        }
    }

    public void AddCentralHall(int a, int b)
    {
        if (a <= 0 || a >= Rows || b <= 0 || b >= Columns) return;
        if (Rows % 2 != a % 2 || Columns % 2 != b % 2) return;

        int startRow = (Rows - a) / 2;
        int startColumn = (Columns - b) / 2;
        for (int i = 0; i < a; i++)
        {
            for (int j = 0; j < b; j++)
            {
                _board[startRow + i, startColumn + j] = new GroundCell();
            }
        }
    }

    public void GenerateMazeDfs()
    {
        Random random = new Random();
        Stack<(int r, int c)> stack = new Stack<(int r, int c)>();

        int startR = 0;
        int startC = 0;

        _board[startR, startC] = new GroundCell();
        stack.Push((startR, startC));

        (int dr, int dc)[] directions = { (-2, 0), (2, 0), (0, -2), (0, 2) };

        while (stack.Count > 0)
        {
            var (currentR, currentC) = stack.Pop();

            List<(int r, int c, int wallR, int wallC)> unvisitedNeighbors = new();

            foreach (var dir in directions)
            {
                int nextR = currentR + dir.dr;
                int nextC = currentC + dir.dc;

                if (nextR >= 0 && nextR < Rows && nextC >= 0 && nextC < Columns)
                {
                    if (!_board[nextR, nextC].IsPassable)
                    {
                        int wallR = currentR + dir.dr / 2;
                        int wallC = currentC + dir.dc / 2;

                        unvisitedNeighbors.Add((nextR, nextC, wallR, wallC));
                    }
                }
            }

            if (unvisitedNeighbors.Count > 0)
            {
                stack.Push((currentR, currentC));

                var next = unvisitedNeighbors[random.Next(unvisitedNeighbors.Count)];

                _board[next.wallR, next.wallC] = new GroundCell();

                _board[next.r, next.c] = new GroundCell();

                stack.Push((next.r, next.c));
            }
        }
    }

    public (int, int) GetRandomField(Random random)
    {
        int row, column;
        do
        {
            row = random.Next(0, Rows);
            column = random.Next(0, Columns);
        } while (!_board[row, column].IsPassable);

        return (row, column);
    }

    public Item GetJunkItem(int index)
    {
        Dictionary<int, Item> itemDict = new Dictionary<int, Item>();

        itemDict[0] = new JunkItem("Old Boot", 'b', 5, "Someone lost it ages ago");
        itemDict[1] = new JunkItem("Skull", 'x', 3, "Empty inside.");
        itemDict[2] = new JunkItem("Broken Compas", 'o', 3, "It only points south.");

        if (index < 0 || index > _numberOfDifferentJunkItems) return itemDict[0];

        return itemDict[index];
    }

    public Item GetWeaponItem(int index)
    {
        Dictionary<int, Item> itemDict = new Dictionary<int, Item>();

        itemDict[0] = new Weapon("Rusty Dagger", '/', 3, damage: 5,
            isTwoHanded: false, "Covered in rust, but still pointy enough to do some damage.");
        itemDict[1] = new Weapon("Steel Sword", 't', 2, damage: 12,
            isTwoHanded: false, "A classic, sharp blade forged by a local blacksmith. Reliable in combat.");
        itemDict[2] = new Weapon("Orcish Greataxe", 'T', 1, damage: 25,
            isTwoHanded: true, "Heavy and devastating. You will need both hands to swing this monster.");

        if (index < 0 || index > _numberOfDifferentWeapons) return itemDict[0];

        return itemDict[index];
    }

    public Item GetCurrencyItem(int index)
    {
        Dictionary<int, Item> itemDict = new Dictionary<int, Item>();

        itemDict[0] = new CoinItem("Coin", 'c', 3, 5,
            "A small leather pouch clinking with standard copper coins.");
        itemDict[1] = new GoldItem("Gold", 'G', 2, 15,
            "A shiny, heavy ingot of solid gold. Merchants will love this.");

        if (index < 0 || index > _numberOfDifferentCurrencies) return itemDict[0];

        return itemDict[index];
    }
}