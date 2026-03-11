namespace RPGGame.Items;

public enum CurrencyType
{
    Coin,
    Gold,
}

public class CurrencyItem(string name, char symbol, int rarity, CurrencyType type, int amount, String description) 
    : Item(name, symbol, rarity, description)
{
    public CurrencyType Type { get; } = type;
    public int Amount { get; } = amount;
    
    public override void OnPickedUp(Player player)
    {
        if (Type == CurrencyType.Coin)
        {
            player.Coins += Amount;
        }
        else if (Type == CurrencyType.Gold)
        {
            player.Gold += Amount;
        }
    }

    public override void Equip(Player player) {}
}