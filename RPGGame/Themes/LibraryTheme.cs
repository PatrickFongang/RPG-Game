using RPGGame.Builder;
using RPGGame.Items;

namespace RPGGame.Themes;

public class LibraryTheme : IDungeonTheme
{
    public string WelcomeMessage => "You step into the Grand Archives. The scent of decaying parchment and ancient dust fills the air." +
                                    " Rogue mages have taken over, guarding the forbidden Black Wand. Knowledge is power, but here, it" +
                                    " might cost you your life.";

    public void ApplyLayoutStrategy(IDungeonBuilder builder)
    {
        builder.BuildFilledDungeon();
        builder.GenerateMazeDfs();
    }

    public Item CreateJunkItem(Random random)
    {
        Item book = new JunkItem("Dusty Book", 'b', 5, "Pages are falling apart.");
        return new WisdomModifierDecorator(book, "Wise", random.Next(1, 4));
    }

    public Item CreateWeaponItem(Random random) 
    {
        Item weapon = new MagicWeapon("Enchanted Scroll", 's', 2, 8, false, "Contains offensive spells.");
        
        if (random.Next(100) < 50) weapon = new WisdomModifierDecorator(weapon, "Ancient", random.Next(2, 6));
        
        if (random.Next(100) < 30) weapon = new DamageModifierDecorator(weapon, "Torn", -2);
        
        return weapon;
    }

    public Item CreateCurrencyItem(Random random) => new CoinItem("Library Token", 'c', 3, 5, "Used to pay late fees.");

    public Item CreateArtifact() => new MagicWeapon("Black Wand", 'W', 1, 20, false, "A powerful wand radiating dark energy.");

    public Enemy CreateEnemy(Random random) => new Enemy("Mage", 'M', health: 40, attack: 15, armor: 2);
}