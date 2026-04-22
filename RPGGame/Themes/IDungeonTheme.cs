using RPGGame.Builder;
using RPGGame.Items;

namespace RPGGame.Themes;

public interface IDungeonTheme
{
    string WelcomeMessage { get; }
    
    void ApplyLayoutStrategy(IDungeonBuilder builder);
    Item CreateJunkItem(Random random);
    Item CreateWeaponItem(Random random);
    Item CreateCurrencyItem(Random random);
    Item CreateArtifact();
    Enemy CreateEnemy(Random random);
}