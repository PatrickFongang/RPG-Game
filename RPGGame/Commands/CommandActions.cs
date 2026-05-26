namespace RPGGame.Commands;

using RPGGame.MVC;
using RPGGame.Items;
using RPGGame.Logging;
using RPGGame.Combat;

public static class CommandActions
{
    public static void PickUp(GameModel model, int playerId)
    {
        if (!model.Players.TryGetValue(playerId, out Player player)) return;
        var cell = model.Dungeon[player.Y, player.X];
        if (cell.ItemsOnGround.Count == 0) return;

        Item item = cell.ItemsOnGround.Pop();
        item.OnPickedUp(player);
        
        EventLogger.Instance.Log($"{player.PlayerName} picked up item: {item.Name}");

        if (item.NoiseRange > 0)
        {
            player.NotifySound(item.NoiseRange, model.Dungeon);
        }
        
        if (item.GoesToBackpack)
        {
            player.Backpack.AddItem(item);
        }
        model.NotifyViews();
    }

    public static void Equip(GameModel model, int playerId)
    {
        if (!model.Players.TryGetValue(playerId, out Player player)) return;
        Item item = player.Backpack.SelectedItem;
        if (item == null || !item.IsEquippable) return;

        if (item.IsTwoHanded)
        {
            if (player.LeftHand == null && player.RightHand == null)
            {
                player.LeftHand = item;
                player.RightHand = item;
                player.Backpack.RemoveItem();
            }
        }
        else
        {
            if (player.LeftHand == null)
            {
                player.LeftHand = item;
                player.Backpack.RemoveItem();
            }
            else if (player.RightHand == null)
            {
                player.RightHand = item;
                player.Backpack.RemoveItem();
            }
        }
        model.NotifyViews();
    }

    public static void Drop(GameModel model, int playerId)
    {
        if (!model.Players.TryGetValue(playerId, out Player player)) return;
        Item item = player.Backpack.SelectedItem;
        if (item == null) return;

        model.Dungeon[player.Y, player.X].ItemsOnGround.Push(item);
        player.Backpack.RemoveItem();
        model.NotifyViews();
    }
    
    public static void FreeLeftHand(GameModel model, int playerId)
    {
        if (!model.Players.TryGetValue(playerId, out Player player)) return;
        Item item = player.LeftHand;
        if (item == null) return;

        EventLogger.Instance.Log($"{player.PlayerName} unequipped item: {item.Name}");

        if (player.LeftHand == player.RightHand)
        {
            player.LeftHand = null;
            player.RightHand = null;
            player.Backpack.AddItem(item);
        }
        else
        {
            player.LeftHand = null;
            player.Backpack.AddItem(item);
        }
        model.NotifyViews();
    }

    public static void FreeRightHand(GameModel model, int playerId)
    {
        if (!model.Players.TryGetValue(playerId, out Player player)) return;
        Item item = player.RightHand;
        if (item == null) return;

        EventLogger.Instance.Log($"{player.PlayerName} unequipped item: {item.Name}");

        if (player.LeftHand == player.RightHand)
        {
            player.LeftHand = null;
            player.RightHand = null;
            player.Backpack.AddItem(item);
        }
        else
        {
            player.RightHand = null;
            player.Backpack.AddItem(item);
        }
        model.NotifyViews();
    }

    public static void CycleAttackStrategy(GameModel model, int playerId)
    {
        if (!model.Players.TryGetValue(playerId, out Player player)) return;

        if (player.CurrentAttackStrategy is NormalAttack)
        {
            player.ChangeAttackStrategy(new StealthAttack());
            EventLogger.Instance.Log($"{player.PlayerName} changed attack to Stealth.");
        }
        else if (player.CurrentAttackStrategy is StealthAttack)
        {
            player.ChangeAttackStrategy(new MagicAttack());
            EventLogger.Instance.Log($"{player.PlayerName} changed attack to Magic.");
        }
        else
        {
            player.ChangeAttackStrategy(new NormalAttack());
            EventLogger.Instance.Log($"{player.PlayerName} changed attack to Normal.");
        }
        model.NotifyViews();
    }

    public static void MoveInventoryUp(GameModel model, int playerId)
    {
        if (!model.Players.TryGetValue(playerId, out Player player)) return;
        player.Backpack.MoveSelectedItemUp();
        model.NotifyViews();
    }

    public static void MoveInventoryDown(GameModel model, int playerId)
    {
        if (!model.Players.TryGetValue(playerId, out Player player)) return;
        player.Backpack.MoveSelectedItemDown();
        model.NotifyViews();
    }
}