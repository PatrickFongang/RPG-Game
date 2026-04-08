namespace RPGGame.Builder;

public interface IDungeonBuilder
{
    void Reset();
    void BuildEmptyDungeon();
    void BuildFilledDungeon();
    void AddRooms(int numberOfRooms);
    void AddJunkItems(int numberOfJunkItems);
    void AddWeapons(int numberOfWeapons);
    void AddCurrencies(int numberOfCurrencies);
    void AddCentralHall(int a, int b);
    void GenerateMazeDfs();
    void AddEnemies(int numberOfEnemies);
    
    Dungeon GetDungeon();
}