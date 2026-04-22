using RPGGame.Builder;
using RPGGame.Themes;

namespace RPGGame.Director;

public class DungeonDirector
{
    private IDungeonBuilder _builder;

    public DungeonDirector(IDungeonBuilder builder)
    {
        _builder = builder;
    }
    public void ConstructThemedDungeon(IDungeonTheme theme)
    {
        _builder.Reset();
        _builder.SetTheme(theme);
        
        theme.ApplyLayoutStrategy(_builder); 
        
        _builder.AddJunkItems(3);
        _builder.AddWeapons(2);
        _builder.AddCurrencies(3);
        _builder.AddEnemies(3);
        _builder.AddArtifact();
    }
}