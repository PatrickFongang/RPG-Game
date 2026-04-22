using RPGGame.Commands;
using RPGGame.Items;
using RPGGame.Combat;
using RPGGame.Logging;

namespace RPGGame;

public class GameEngine
{
    public bool IsGameOver { get; private set; } = false;
    
    private readonly Dungeon _dungeon;
    private readonly Player _player;
    private readonly Render _render;
    private Dictionary<ConsoleKey, ICommand> _commands;
    
    public ConsoleKey KeyMoveUp { get; } = ConsoleKey.W;
    public ConsoleKey KeyMoveDown { get; } = ConsoleKey.S;
    public ConsoleKey KeyMoveLeft { get; } = ConsoleKey.A;
    public ConsoleKey KeyMoveRight { get; } = ConsoleKey.D;
    public ConsoleKey KeyPickUp { get; } = ConsoleKey.E;
    public ConsoleKey KeyEquip { get; } = ConsoleKey.Q;
    public ConsoleKey KeyDrop { get; } = ConsoleKey.T;
    public ConsoleKey KeyFreeLeftHand { get; } = ConsoleKey.L;
    public ConsoleKey KeyFreeRightHand { get; } = ConsoleKey.R;
    public ConsoleKey KeyChangeStrategy { get; } = ConsoleKey.Z;
    public ConsoleKey KeyViewLogs { get; } = ConsoleKey.J;

    public GameEngine(Dungeon dungeon)
    {
        _dungeon = dungeon;
        _player = new Player(dungeon.Columns / 2, _dungeon.Rows / 2);
        _render = new Render(_dungeon, _player, this);

        InitializeCommands();
    }

    private void InitializeCommands()
    {
        _commands = new Dictionary<ConsoleKey, ICommand>
        {
            { KeyMoveUp, new MoveCommand(_player, _dungeon, this, 0, -1) },
            { KeyMoveDown, new MoveCommand(_player, _dungeon, this, 0, 1) },
            { KeyMoveLeft, new MoveCommand(_player, _dungeon, this, -1, 0) },
            { KeyMoveRight, new MoveCommand(_player, _dungeon, this, 1, 0) },
            { KeyPickUp, new ActionCommand(TryToPickUpItem) },
            { KeyEquip, new ActionCommand(TryToEquipItem) },
            { ConsoleKey.DownArrow, new ActionCommand(_player.Backpack.MoveSelectedItemDown) },
            { ConsoleKey.UpArrow, new ActionCommand(_player.Backpack.MoveSelectedItemUp) },
            { KeyDrop, new ActionCommand(TryToThrowItem) },
            { KeyFreeLeftHand, new ActionCommand(TryToFreeLeftHand) },
            { KeyFreeRightHand, new ActionCommand(TryToFreeRightHand) },
            { KeyChangeStrategy, new ActionCommand(CycleAttackStrategy) },
            { KeyViewLogs, new ActionCommand(ShowFullLogs) }
        };
    }

    public void StartGame()
    {
        Console.CursorVisible = false;
        if (OperatingSystem.IsWindows())
        {
            Console.WindowWidth = 120;
            Console.BufferWidth = 120;

            Console.WindowHeight = 40;
            Console.BufferHeight = 40;
        }

        RunLogic();
    }

    public void ResolveCombat(Enemy enemy, Cell cellWithEnemy)
    {
        IAttackVisitor attack = _player.CurrentAttackStrategy;


        int leftHandDamage = _player.LeftHand != null 
            ? _player.LeftHand.CalculateDamageWith(attack, _player) + _player.LeftHand.GetDamageModifier()
            : 0;

        int rightHandDamage = (_player.RightHand != null && _player.RightHand != _player.LeftHand) 
            ? _player.RightHand.CalculateDamageWith(attack, _player) + _player.RightHand.GetDamageModifier() 
            : 0;

        int totalPlayerDamage = leftHandDamage + rightHandDamage;

        int damageDealtToEnemy = Math.Max(0, totalPlayerDamage - enemy.Armor);
        enemy.Health -= damageDealtToEnemy;

        EventLogger.Instance.Log($"{_player.PlayerName} attacked {enemy.Name} dealing {damageDealtToEnemy} damage.");

        if (enemy.Health <= 0)
        {
            EventLogger.Instance.Log($"{_player.PlayerName} defeated the {enemy.Name}.");
            cellWithEnemy.Enemy = null;
            return;
        }


        int leftHandDefense = _player.LeftHand != null
            ? _player.LeftHand.CalculateDefenseWith(attack, _player)
            : attack.CalculateDefense((Item)null, _player);
        int rightHandDefense = (_player.RightHand != null && _player.RightHand != _player.LeftHand)
            ? _player.RightHand.CalculateDefenseWith(attack, _player)
            : 0;

        if (_player.LeftHand == null && _player.RightHand == null)
        {
            leftHandDefense = attack.CalculateDefense((Item)null, _player);
        }

        int totalPlayerDefense = leftHandDefense + rightHandDefense;

        int damageDealtToPlayer = Math.Max(0, enemy.Attack - totalPlayerDefense);
        _player.Health -= damageDealtToPlayer;

        EventLogger.Instance.Log($"Enemy {enemy.Name} attacked {_player.PlayerName} dealing {damageDealtToPlayer} damage.");

        if (_player.Health <= 0)
        {
            IsGameOver = true;
        }
    }

    private void RunLogic()
    {
        ConsoleKeyInfo cki;
        do
        {
            _render.RenderUI();
            
            if (IsGameOver)
            {
                break; 
            }

            cki = Console.ReadKey(true);

            if (_commands.TryGetValue(cki.Key, out ICommand? command))
            {
                command.Execute();
            }
            else
            {
                EventLogger.Instance.Log($"{_player.PlayerName} pressed unknown button: {cki.Key}");
            }
        } while (true);
        
        _render.RenderGameOverScreen();
        Console.ReadKey(true);
        Environment.Exit(0);
    }

    private void TryToFreeLeftHand()
    {
        Item? item = _player.LeftHand;
        if (item == null) return;

        EventLogger.Instance.Log($"{_player.PlayerName} unequipped item: {item.Name}");

        if (_player.LeftHand == _player.RightHand)
        {
            _player.LeftHand = null;
            _player.RightHand = null;
            _player.Backpack.AddItem(item);
        }
        else
        {
            _player.LeftHand = null;
            _player.Backpack.AddItem(item);
        }
    }

    private void TryToFreeRightHand()
    {
        Item? item = _player.RightHand;
        if (item == null) return;

        EventLogger.Instance.Log($"{_player.PlayerName} unequipped item: {item.Name}");

        if (_player.LeftHand == _player.RightHand)
        {
            _player.LeftHand = null;
            _player.RightHand = null;
            _player.Backpack.AddItem(item);
        }
        else
        {
            _player.RightHand = null;
            _player.Backpack.AddItem(item);
        }
    }

    private void TryToPickUpItem()
    {
        Cell currentCell = _dungeon[_player.Y, _player.X];
        if (currentCell.ItemsOnGround.Count == 0) return;

        Item item = currentCell.ItemsOnGround.Pop();

        item.OnPickedUp(_player);
        EventLogger.Instance.Log($"{_player.PlayerName} picked up item: {item.Name}");

        if (item.GoesToBackpack)
        {
            _player.Backpack.AddItem(item);
        }
    }

    private void TryToEquipItem()
    {
        Item? item = _player.Backpack.SelectedItem;
        if (item == null || !item.IsEquippable) return;

        EventLogger.Instance.Log($"{_player.PlayerName} equipped item: {item.Name}");

        if (item.IsTwoHanded)
        {
            if (_player.LeftHand == null && _player.RightHand == null)
            {
                _player.LeftHand = item;
                _player.RightHand = item;
                _player.Backpack.RemoveItem();
            }
        }
        else
        {
            if (_player.LeftHand == null)
            {
                _player.LeftHand = item;
                _player.Backpack.RemoveItem();
            }
            else if (_player.RightHand == null)
            {
                _player.RightHand = item;
                _player.Backpack.RemoveItem();
            }
        }
    }

    private void TryToThrowItem()
    {
        Item? item = _player.Backpack.SelectedItem;
        if (item == null) return;

        _dungeon[_player.Y, _player.X].ItemsOnGround.Push(item);
        _player.Backpack.RemoveItem();
    }

    private void CycleAttackStrategy()
    {
        if (_player.CurrentAttackStrategy is NormalAttack)
        {
            _player.ChangeAttackStrategy(new StealthAttack());
            EventLogger.Instance.Log($"{_player.PlayerName} changed attack to Stealth.");
        }
        else if (_player.CurrentAttackStrategy is StealthAttack)
        {
            _player.ChangeAttackStrategy(new MagicAttack());
            EventLogger.Instance.Log($"{_player.PlayerName} changed attack to Magic.");
        }
        else
        {
            _player.ChangeAttackStrategy(new NormalAttack());
            EventLogger.Instance.Log($"{_player.PlayerName} changed attack to Normal.");
        }
    }

    private void ShowFullLogs()
    {
        _render.RenderFullLogsScreen();
    }
}