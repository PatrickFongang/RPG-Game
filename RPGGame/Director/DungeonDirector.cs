using RPGGame.Builder;

namespace RPGGame.Director;

public class DungeonDirector
{
    private IDungeonBuilder _builder;

    public DungeonDirector(IDungeonBuilder builder)
    {
        _builder = builder;
    }

    public void ChangeBuilder(IDungeonBuilder builder)
    {
        _builder = builder;
    }

    public void ConstructStandardDungeon()
    {
        _builder.Reset();
        _builder.BuildEmptyDungeon();
        _builder.BuildFilledDungeon();
        _builder.GenerateMazeDfs();
        _builder.AddCentralHall(4, 8);
        _builder.AddJunkItems(2);
        _builder.AddWeapons(2);
        _builder.AddCurrencies(2);
    }

    public void ConstructEmptyRoom()
    {
        _builder.Reset();
        _builder.BuildEmptyDungeon();
        _builder.AddCurrencies(5);
    }

    public void ConstructStandardDungeonWithEnemies()
    {
        _builder.Reset();
        _builder.BuildEmptyDungeon();
        _builder.BuildFilledDungeon();
        _builder.GenerateMazeDfs();
        _builder.AddCentralHall(4, 8);
        _builder.AddJunkItems(2);
        _builder.AddWeapons(2);
        _builder.AddCurrencies(2);
        _builder.AddEnemies(2);
    }
}