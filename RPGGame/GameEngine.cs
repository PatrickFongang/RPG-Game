using RPGGame.Commands;
using RPGGame.Items;
using RPGGame.Strategies;

namespace RPGGame;

public class GameEngine
{
    private Builder _builder;
    private Player _player;
    private Render _render;
    private Dictionary<ConsoleKey, ICommand> _commands;

    public string LastMessage { get; set; } = "";

    public GameEngine(IDungeonStrategy strategy)
    {
        _builder = new Builder(20, 40);
        _player = new Player(_builder.Columns / 2, _builder.Rows / 2);
        _render = new Render(_builder, _player, this);

        strategy.Generate(_builder);
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        _commands = new Dictionary<ConsoleKey, ICommand>
        {
            { ConsoleKey.W, new MoveCommand(_player, _builder, 0, -1) },
            { ConsoleKey.S, new MoveCommand(_player, _builder, 0, 1) },
            { ConsoleKey.A, new MoveCommand(_player, _builder, -1, 0) },
            { ConsoleKey.D, new MoveCommand(_player, _builder, 1, 0) },
            { ConsoleKey.E, new ActionCommand(TryToPickUpItem) },
            { ConsoleKey.Q, new ActionCommand(TryToEquipItem) },
            { ConsoleKey.DownArrow, new ActionCommand(_player.Backpack.MoveSelectedItemDown) },
            { ConsoleKey.UpArrow, new ActionCommand(_player.Backpack.MoveSelectedItemUp) },
            { ConsoleKey.T, new ActionCommand(TryToThrowItem) },
            { ConsoleKey.L, new ActionCommand(TryToFreeLeftHand) },
            { ConsoleKey.R, new ActionCommand(TryToFreeRightHand) }
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

    public void RunLogic()
    {
        ConsoleKeyInfo cki;
        do
        {
            _render.RenderUI();

            cki = Console.ReadKey(true);
            LastMessage = "";

            if (_commands.TryGetValue(cki.Key, out ICommand? command))
            {
                command.Execute();
            }
            else
            {
                LastMessage = "Unknown command! Press the right key.";
            }
        } while (true);
    }

    public void TryToFreeLeftHand()
    {
        Item? item = _player.LeftHand;
        if (item == null) return;

        if (_player.LeftHand == _player.RightHand)
        {
            _player.LeftHand = null;
            _player.RightHand = null;
            item.OnPickedUp(_player);
        }
        else
        {
            _player.LeftHand = null;
            item.OnPickedUp(_player);
        }
    }

    public void TryToFreeRightHand()
    {
        Item? item = _player.RightHand;
        if (item == null) return;

        if (_player.LeftHand == _player.RightHand)
        {
            _player.LeftHand = null;
            _player.RightHand = null;
            item.OnPickedUp(_player);
        }
        else
        {
            _player.RightHand = null;
            item.OnPickedUp(_player);
        }
    }

    public void TryToEquipItem()
    {
        Item? item = _player.Backpack.SelectedItem;
        if (item == null) return;

        item.Equip(_player);
    }

    public void TryToPickUpItem()
    {
        Cell currentCell = _builder[_player.Y, _player.X];

        if (currentCell.ItemsOnGround.Count == 0) return;

        Item item = currentCell.ItemsOnGround.Pop();

        item.OnPickedUp(_player);
    }

    public void TryToThrowItem()
    {
        Item? item = _player.Backpack.SelectedItem;
        if (item == null) return;

        _builder[_player.Y, _player.X].ItemsOnGround.Push(item);
        _player.Backpack.RemoveItem();
    }
}