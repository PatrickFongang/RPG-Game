namespace RPGGame.Strategies;

public class StandardDungeonStrategy : IDungeonStrategy
{
    public void Generate(Builder builder)
    {
        builder.BuildEmptyDungeon();
        builder.BuildFilledDungeon();
        builder.GenerateMazeDfs();
        builder.AddCentralHall(4, 8);
        builder.AddJunkItems(2);
        builder.AddWeapons(2);
        builder.AddCurrencies(2);
    }
}