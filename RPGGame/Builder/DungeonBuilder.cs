using System.Diagnostics;
using System.Runtime.InteropServices.Swift;
using RPGGame.Items;
using RPGGame.Themes;

namespace RPGGame.Builder;

public class DungeonBuilder : IDungeonBuilder
{
    private Random random = new Random();
    private IDungeonTheme? _theme;

    private Dungeon _dungeon;
    private int _rows;
    private int _columns;
    private int _numberOfWalls;

    private readonly int _numberOfDifferentJunkItems = 3;
    private readonly int _numberOfDifferentWeapons = 3;
    private readonly int _numberOfDifferentCurrencies = 2;

    public DungeonBuilder(int rows, int columns)
    {
        _rows = rows;
        _columns = columns;
        Reset();
    }
    public void SetTheme(IDungeonTheme theme)
    {
        _theme = theme;
    }
    public void Reset()
    {
        _dungeon = new Dungeon(_rows, _columns);
        _numberOfWalls = 0;
    }

    public void BuildEmptyDungeon()
    {
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                _dungeon[i, j] = new GroundCell();
            }
        }
    }

    public void BuildFilledDungeon()
    {
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                _dungeon[i, j] = new WallCell();
            }
        }

        _numberOfWalls = _rows * _columns;
        AddCentralHall(1,1);
    }

    public void AddRooms(int numberOfRooms)
    {
        if (numberOfRooms < 1) return;

        for (int i = 0; i < numberOfRooms; i++)
        {
            int roomWidth = random.Next(4, 10);
            int roomHeight = random.Next(3, 7);

            int startRow = random.Next(1, Math.Max(2, _rows - roomHeight - 1));
            int startCol = random.Next(1, Math.Max(2, _columns - roomWidth - 1));

            for (int r = startRow; r < startRow + roomHeight && r < _rows - 1; r++)
            {
                for (int c = startCol; c < startCol + roomWidth && c < _columns - 1; c++)
                {
                    _dungeon[r, c] = new GroundCell();
                }
            }
        }
    }

    public void AddJunkItems(int numberOfJunkItems)
    {
        if (numberOfJunkItems < 1 || _theme == null) return;
        _dungeon.HasPickups = true;
        _dungeon.HasInventoryItems = true;

        for (int i = 0; i < numberOfJunkItems; i++)
        {
            (int row, int col) field = GetRandomField();
            _dungeon[field.row, field.col].ItemsOnGround.Push(_theme.CreateJunkItem(random));
        }
    }

    public void AddWeapons(int numberOfWeapons)
    {
        if (numberOfWeapons < 1 || _theme == null) return;
        _dungeon.HasPickups = true;
        _dungeon.HasInventoryItems = true;

        for (int i = 0; i < numberOfWeapons; i++)
        {
            (int row, int col) field = GetRandomField();
            _dungeon[field.row, field.col].ItemsOnGround.Push(_theme.CreateWeaponItem(random));
        }
    }

    public void AddCurrencies(int numberOfCurrencies)
    {
        if (numberOfCurrencies < 1 || _theme == null) return;
        _dungeon.HasPickups = true;

        for (int i = 0; i < numberOfCurrencies; i++)
        {
            (int row, int col) field = GetRandomField();
            _dungeon[field.row, field.col].ItemsOnGround.Push(_theme.CreateCurrencyItem(random));
        }
    }

    public void AddArtifact()
    {
        if (_theme == null) return;
        _dungeon.HasPickups = true;
        _dungeon.HasInventoryItems = true;
        
        (int row, int col) field = GetRandomField();
        _dungeon[field.row, field.col].ItemsOnGround.Push(_theme.CreateArtifact());
    }

    public void AddEnemies(int numberOfEnemies)
    {
        if (numberOfEnemies < 1 || _theme == null) return;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int row, column;
            do
            {
                (row, column) = GetRandomField();
            } while (_dungeon[row, column].Enemy != null);

            _dungeon[row, column].Enemy = _theme.CreateEnemy(random);
        }
    }

    public void AddCentralHall(int a, int b)
    {
        if (a <= 0 || a >= _rows || b <= 0 || b >= _columns) return;
        if (_rows % 2 != a % 2 || _columns % 2 != b % 2) return;

        int startRow = (_rows - a) / 2;
        int startColumn = (_columns - b) / 2;
        for (int i = 0; i < a; i++)
        {
            for (int j = 0; j < b; j++)
            {
                _dungeon[startRow + i, startColumn + j] = new GroundCell();
            }
        }
    }

    public void GenerateMazeDfs()
    {
        Stack<(int r, int c)> stack = new Stack<(int r, int c)>();

        int startR = 0;
        int startC = 0;

        _dungeon[startR, startC] = new GroundCell();
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

                if (nextR >= 0 && nextR < _rows && nextC >= 0 && nextC < _columns)
                {
                    if (!_dungeon[nextR, nextC].IsPassable)
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
                _dungeon[next.wallR, next.wallC] = new GroundCell();
                _dungeon[next.r, next.c] = new GroundCell();
                stack.Push((next.r, next.c));
            }
        }
    }

    public Dungeon GetDungeon()
    {
        Dungeon result = _dungeon;
        this.Reset();
        return result;
    }

    private (int, int) GetRandomField()
    {
        int row, column;
        do
        {
            row = random.Next(0, _rows);
            column = random.Next(0, _columns);
        } while (!_dungeon[row, column].IsPassable);

        return (row, column);
    }
}