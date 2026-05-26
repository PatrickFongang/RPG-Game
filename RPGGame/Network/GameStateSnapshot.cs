namespace RPGGame.Network;

using System;
using System.Linq;
using RPGGame.MVC;
using RPGGame.Network.DTOs;
using RPGGame.Items;
using RPGGame.Logging;

public static class GameStateSnapshot
{
    public static GameStateDTO CreateSnapshot(GameModel model)
    {
        return new GameStateDTO
        {
            Rows = model.Dungeon.Rows,
            Columns = model.Dungeon.Columns,
            IsGameOver = model.IsGameOver,
            HasPickups = model.Dungeon.HasPickups,
            HasInventoryItems = model.Dungeon.HasInventoryItems,
            Players = model.Dungeon.ActiveEnemies.Count > -1 ? model.Players.ToDictionary(
                kvp => kvp.Key,
                kvp => MapPlayer(kvp.Key, kvp.Value)
            ) : new(),
            Enemies = model.Dungeon.ActiveEnemies.Select(MapEnemy).ToList(),
            Board = MapBoard(model.Dungeon),
            LatestLogs = EventLogger.Instance.GetLogs().ToList() 
        };
    }

    private static PlayerDTO MapPlayer(int id, Player player)
    {
        return new PlayerDTO
        {
            Id = id,
            Name = player.PlayerName,
            X = player.X,
            Y = player.Y,
            Health = player.Health,
            Wallet = player.Wallet.ToDictionary(k => k.Key, v => v.Value),
            LeftHand = MapItem(player.LeftHand),
            RightHand = MapItem(player.RightHand),
            BaseStrength = player.BaseStrength,
            BaseAgility = player.BaseAgility,
            Wisdom = player.Wisdom,
            Luck = player.Luck,
            Aggression = player.Aggression,
            AttackStrategyName = player.CurrentAttackStrategy.GetType().Name.Replace("Attack", "").ToUpper(),
            Backpack = player.Backpack.Items.Select(MapItem).ToList(),
            SelectedItemIndex = player.Backpack.SelectedItemIndex
        };
    }

    private static EnemyDTO MapEnemy(Enemy enemy)
    {
        return new EnemyDTO
        {
            Name = enemy.Name,
            Symbol = enemy.Symbol,
            X = enemy.X,
            Y = enemy.Y,
            Health = enemy.Health
        };
    }

    private static ItemDTO? MapItem(Item? item)
    {
        if (item == null) return null;
        return new ItemDTO
        {
            Name = item.Name,
            Symbol = item.Symbol,
            Description = item.Description
        };
    }

    private static CellDTO[][] MapBoard(Dungeon dungeon)
    {
        var board = new CellDTO[dungeon.Rows][];
        for (int i = 0; i < dungeon.Rows; i++)
        {
            board[i] = new CellDTO[dungeon.Columns];
            for (int j = 0; j < dungeon.Columns; j++)
            {
                var cell = dungeon[i, j];
                ItemDTO? topItem = cell.ItemsOnGround.Count > 0 ? MapItem(cell.ItemsOnGround.Peek()) : null;
                
                char symbol = topItem != null ? topItem.Symbol : cell.GetSymbol();
                ConsoleColor color = topItem != null ? ConsoleColor.Blue : cell.GetColor();

                board[i][j] = new CellDTO
                {
                    Symbol = symbol,
                    ForegroundColor = (int)color,
                    TopItemOnGround = topItem
                };
            }
        }
        return board;
    }
}