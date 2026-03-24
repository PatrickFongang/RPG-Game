namespace RPGGame.Strategies;

public class EmptyRoomStrategy : IDungeonStrategy
{
    public void Generate(Builder builder)
    {
        builder.BuildEmptyDungeon();
        builder.AddCurrencies(5);
    }
}