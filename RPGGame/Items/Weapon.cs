namespace RPGGame.Items;

public class Weapon(string name, char symbol, int rarity, int damage, bool isTwoHanded, String description)
    : Item(name, symbol, rarity, description)
{
    public int Damage { get; set; } = damage;
    public bool IsTwoHanded { get; set; } = isTwoHanded;

    public override void OnPickedUp(Player player)
    {
        player.Backpack.AddItem(this);
    }

    public override void Equip(Player player)
    {
        if (this.IsTwoHanded)
        {
            if (player.LeftHand == null && player.RightHand == null)
            {
                player.LeftHand = this;
                player.RightHand = this;
                player.Backpack.RemoveItem();
            }
        }
        else if (player.LeftHand == null)
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