namespace RPGGame.Items;

public abstract class CurrencyItem(string name, char symbol, int rarity, int amount, string description) 
    : Item(name, symbol, rarity, description)
{
    public int Amount { get; } = amount;
    
    public override bool IsEquippable => false;
    public override bool GoesToBackpack => false;
}

public class CoinItem(string name, char symbol, int rarity, int amount, string description) 
    : CurrencyItem(name, symbol, rarity, amount, description)
{
    public override void OnPickedUp(Player player)
    {
        player.Coins += Amount; 
    }
}

public class GoldItem(string name, char symbol, int rarity, int amount, string description)
    : CurrencyItem(name, symbol, rarity, amount, description)
{
    public override void OnPickedUp(Player player)
    {
        player.Gold += Amount;
    }
}