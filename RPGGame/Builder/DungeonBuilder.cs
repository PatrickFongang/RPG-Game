using System.Diagnostics;
using System.Runtime.InteropServices.Swift;
using RPGGame.Items;

namespace RPGGame.Builder;

public class DungeonBuilder : IDungeonBuilder
{
    private Random random = new Random();

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
    }

    public void AddRooms(int numberOfRooms)
    {
        if (numberOfRooms < 1 || numberOfRooms > _numberOfWalls) return;

        for (int i = 0; i < numberOfRooms; i++)
        {
            int row, column;
            do
            {
                row = random.Next(0, _rows);
                column = random.Next(0, _columns);
            } while (_dungeon[row, column].IsPassable);

            _dungeon[row, column] = new GroundCell();
            _numberOfWalls--;
        }
    }

    public void AddJunkItems(int numberOfJunkItems)
    {
        if (numberOfJunkItems < 1) return;
        _dungeon.HasPickups = true;
        _dungeon.HasInventoryItems = true;

        for (int i = 0; i < numberOfJunkItems; i++)
        {
            (int row, int col) field = GetRandomField();
            int index = random.Next(0, _numberOfDifferentJunkItems);
            _dungeon[field.row, field.col].ItemsOnGround.Push(GetJunkItem(index));
        }
    }

    public void AddWeapons(int numberOfWeapons)
    {
        if (numberOfWeapons < 1) return;
        _dungeon.HasPickups = true;
        _dungeon.HasInventoryItems = true;

        for (int i = 0; i < numberOfWeapons; i++)
        {
            (int row, int col) field = GetRandomField();
            int index = random.Next(0, _numberOfDifferentWeapons);
            _dungeon[field.row, field.col].ItemsOnGround.Push(GetWeaponItem(index));
        }
    }

    public void AddCurrencies(int numberOfCurrencies)
    {
        if (numberOfCurrencies < 1) return;
        _dungeon.HasPickups = true;

        for (int i = 0; i < numberOfCurrencies; i++)
        {
            (int row, int col) field = GetRandomField();
            int index = random.Next(0, _numberOfDifferentCurrencies);
            _dungeon[field.row, field.col].ItemsOnGround.Push(GetCurrencyItem(index));
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

    public void AddEnemies(int numberOfEnemies)
    {
        if (numberOfEnemies < 1) return;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int row, column;
            do
            {
                (row, column) = GetRandomField();
            } while (_dungeon[row, column].Enemy != null);

            int enemyType = random.Next(0, 2);
            Enemy newEnemy = enemyType switch
            {
                0 => new Enemy("Goblin", 'g', health: 30, attack: 12, armor: 3),
                _ => new Enemy("Orc", 'O', health: 50, attack: 18, armor: 5)
            };

            _dungeon[row, column].Enemy = newEnemy;
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

    private Item GetJunkItem(int index)
    {
        Item junkItem = index switch
        {
            0 => new JunkItem("Old Boot", 'b', 5, "Someone lost it ages ago"),
            1 => new JunkItem("Skull", 'x', 3, "Empty inside."),
            _ => new JunkItem("Broken Compas", 'o', 3, "It only points south.")
        };
        if (random.Next(100) < 20)
        {
            junkItem = new LuckModifierDecorator(junkItem, "Unlucky", -3);
        }
        else if (random.Next(100) < 40)
        {
            junkItem = new LuckModifierDecorator(junkItem, "Lucky", 4);
        }

        return junkItem;
    }

    private Item GetWeaponItem(int index)
    {
        Item weapon = index switch
        {
            0 => new LightWeapon("Rusty Dagger", '/', 3, 5, false, "Short but sharp."),
            1 => new LightWeapon("Steel Sword", 't', 2, 12, false, "A classic forged by a blacksmith."),
            _ => new HeavyWeapon("Orcish Greataxe", 'T', 1, 25, true, "Requires both hands to swing.")
        };

        if (random.Next(100) < 50)
        {
            weapon = new DamageModifierDecorator(weapon, "Strong", 5);
        }

        if (random.Next(100) < 30)
        {
            weapon = new LuckModifierDecorator(weapon, "Unlucky", -3);
        }
        else if (random.Next(100) < 15)
        {
            weapon = new LuckModifierDecorator(weapon, "Lucky", 4);
        }

        return weapon;
    }

    private Item GetCurrencyItem(int index)
    {

        Item currItem = index switch
        {
            0 => new CoinItem("Coin", 'c', 3, 5,
                "A small leather pouch clinking with standard copper coins."),
            _ => new GoldItem("Gold", 'G', 2, 15,
                "A shiny, heavy ingot of solid gold. Merchants will love this.")
        };

        return currItem;
    }
}