using RPGGame.Builder;
using RPGGame.Items;

namespace RPGGame.Themes;

public class WealthTheme : IDungeonTheme
{
    public string WelcomeMessage => "A shimmering golden light blinds you momentarily. You have entered the legendary Vault of Greed." +
                                    " The floor is slick with dropped coins, and even the safes have come alive to protect their contents." +
                                    " Grab the Lucky Coin Pouch, but beware your own greed.";

    public void ApplyLayoutStrategy(IDungeonBuilder builder)
    {
        builder.BuildFilledDungeon();
        builder.GenerateMazeDfs();
        builder.AddCentralHall(8, 12); 
    }

    public Item CreateJunkItem(Random random) => new JunkItem("Empty Wallet", 'w', 5, "Someone got here first.");

    public Item CreateWeaponItem(Random random) 
    {
        Item weapon = new HeavyWeapon("Gold Bar", 'b', 2, 15, false, "Heavy enough to crush a skull.");
        
        if (random.Next(100) < 60) weapon = new LuckModifierDecorator(weapon, "Fortunate", random.Next(2, 6));
        
        if (random.Next(100) < 40) weapon = new DamageModifierDecorator(weapon, "Heavy", random.Next(2, 5));
        
        return weapon;
    }

    public Item CreateCurrencyItem(Random random) => random.Next(2) == 0 ? 
        new CoinItem("Gold Coin", 'c', 3, 10, "Shiny gold coin.") : 
        new GoldItem("Gold Ingot", 'G', 2, 50, "Pure gold.");

    public Item CreateArtifact() => new HeavyWeapon("Lucky Coin Pouch", 'P', 1, 20, true, "A heavy sack of coins used as a flail.");

    public Enemy CreateEnemy(Random random) => random.Next(2) == 0 ? 
        new Enemy("Aggressive Briefcase", 'A', health: 35, attack: 10, armor: 4) : 
        new Enemy("Animated Safe", 'S', health: 80, attack: 20, armor: 10);
}