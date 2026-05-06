using RPGGame.Builder;
using RPGGame.Items;
using RPGGame.Observers;

namespace RPGGame.Themes;

public class SciFiTheme : IDungeonTheme
{
    public string WelcomeMessage => "The heavy steel doors hiss shut behind you. The screech of grinding metal and sparking wires" +
                                    " echoes through the sterile corridors. Malfunctioning cleaning bots patrol the area," +
                                    " obliterating any organic waste. Find the Blaster and survive.";

    public void ApplyLayoutStrategy(IDungeonBuilder builder)
    {
        builder.BuildFilledDungeon();
        builder.GenerateMazeDfs();
        builder.AddRooms(5);
    }

    public Item CreateJunkItem(Random random) => new JunkItem("Metal Shard", 'm', 5, "A sharp piece of scrap.");

    public Item CreateWeaponItem(Random random) 
    {
        Item weapon = new LightWeapon("Laser Cutter", 'l', 3, 10, false, "Used for mining.");
        
        if (random.Next(100) < 50) weapon = new DamageModifierDecorator(weapon, "Overcharged", random.Next(3, 8));
        
        if (random.Next(100) < 30) weapon = new LuckModifierDecorator(weapon, "Unstable", -random.Next(1, 4));
        
        return weapon;
    }

    public Item CreateCurrencyItem(Random random) => new GoldItem("Energy Cell", 'E', 2, 10, "Valuable power source.");

    public Item CreateArtifact() => new HeavyWeapon("Blaster", 'B', 1, 25, true, "A high-tech plasma rifle.");

    public IEnumerable<Enemy> CreateEnemyGroup(Random random, int groupSize)
    {
        var group = new List<Enemy>();
        var speciesGroup = new SpeciesGroup();
        
        bool isBotGroup = random.Next(2) == 0;

        for (int i = 0; i < groupSize; i++)
        {
            if (isBotGroup)
            {
                group.Add(new CleaningBot(speciesGroup));
            }
            else
            {
                group.Add(new SecurityDrone(speciesGroup));
            }
        }

        return group;
    }
}