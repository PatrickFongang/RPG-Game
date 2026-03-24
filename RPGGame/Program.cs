
using System.Globalization;
using RPGGame;
using RPGGame.Strategies;

Console.OutputEncoding = System.Text.Encoding.UTF8;

GameEngine newGameEngine = new GameEngine(new StandardDungeonStrategy());
newGameEngine.StartGame();