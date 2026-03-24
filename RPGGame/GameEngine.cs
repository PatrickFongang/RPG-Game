using RPGGame.Items;

namespace RPGGame;

public class GameEngine
{
    private Builder _builder;
    private Player _player;
    private Render _render;

    public GameEngine()
    {
        _builder = new Builder(20,40);
        _player = new Player(_builder.Columns / 2, _builder.Rows / 2);
        _render = new Render(_builder, _player);
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

        _builder.BuildDungeon();
        RunLogic();
    }

    public void RunLogic()
    {
        ConsoleKeyInfo cki;
        do
        {
            _render.RenderUI();

            cki = Console.ReadKey(true);
            switch (cki.Key)
            {
                case ConsoleKey.W:
                    TryMovePlayerUp();
                    break;
                case ConsoleKey.S:
                    TryMovePlayerDown();
                    break;
                case ConsoleKey.A:
                    TryMovePlayerLeft();
                    break;
                case ConsoleKey.D:
                    TryMovePlayerRight();
                    break;
                case ConsoleKey.E:
                    TryToPickUpItem();
                    break;
                case ConsoleKey.Q:
                    TryToEquipItem();
                    break;
                case ConsoleKey.DownArrow:
                    _player.Backpack.MoveSelectedItemDown();
                    break;
                case ConsoleKey.UpArrow:
                    _player.Backpack.MoveSelectedItemUp();
                    break;
                case ConsoleKey.T:
                    TryToThrowItem();
                    break;
                case ConsoleKey.L:
                    TryToFreeLeftHand();
                    break;
                case ConsoleKey.R:
                    TryToFreeRightHand();
                    break;
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

    public void TryMovePlayerUp()
    {
        int targetX = _player.X;
        int targetY = _player.Y - 1;

        if (targetY < 0) return;

        if (!_builder[targetY, targetX].IsPassable) return;

        _player.Y = targetY;
    }

    public void TryMovePlayerDown()
    {
        int targetX = _player.X;
        int targetY = _player.Y + 1;

        if (targetY + 1 > _builder.Rows) return;

        if (!_builder[targetY, targetX].IsPassable) return;

        _player.Y = targetY;
    }

    public void TryMovePlayerLeft()
    {
        int targetX = _player.X - 1;
        int targetY = _player.Y;

        if (targetX < 0) return;

        if (!_builder[targetY, targetX].IsPassable) return;

        _player.X = targetX;
    }

    public void TryMovePlayerRight()
    {
        int targetX = _player.X + 1;
        int targetY = _player.Y;

        if (targetX + 1 > _builder.Columns) return;

        if (!_builder[targetY, targetX].IsPassable) return;

        _player.X = targetX;
    }
}