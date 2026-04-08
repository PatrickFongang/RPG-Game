using System.Globalization;
using RPGGame;
using RPGGame.Builder;
using RPGGame.Director;

Console.OutputEncoding = System.Text.Encoding.UTF8;

DungeonBuilder builder = new DungeonBuilder(20, 40);
DungeonDirector director = new DungeonDirector(builder);

director.ConstructStandardDungeonWithEnemies();
Dungeon generatedDungeon = builder.GetDungeon();

GameEngine newGameEngine = new GameEngine(generatedDungeon);
newGameEngine.StartGame();