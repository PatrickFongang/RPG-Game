using RPGGame.Themes;

namespace RPGGame.Builder;

public interface IDungeonBuilder
{
    void Reset();
    void SetTheme(IDungeonTheme theme);
    void BuildEmptyDungeon();
    void BuildFilledDungeon();
    void AddRooms(int numberOfRooms);
    void AddJunkItems(int numberOfJunkItems);
    void AddWeapons(int numberOfWeapons);
    void AddCurrencies(int numberOfCurrencies);
    void AddArtifact();
    void AddCentralHall(int a, int b);
    void GenerateMazeDfs();
    void AddEnemies(int numberOfEnemies);
    
    Dungeon GetDungeon();
}