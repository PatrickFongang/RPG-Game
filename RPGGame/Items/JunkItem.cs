namespace RPGGame.Items;

public class JunkItem(string name, char symbol, int rarity, string description)
    : Item(name, symbol, rarity, description)
{
    public override void OnPickedUp(Player player)
    {
        player.Backpack.AddItem(this);
    }

    public override void Equip(Player player)
    {
        if (player.LeftHand == null)
        {
            player.LeftHand = this;
            player.Backpack.RemoveItem();
        }
        else if (player.RightHand == null)
        {
            player.RightHand = this;
            player.Backpack.RemoveItem();
        }
    }
}