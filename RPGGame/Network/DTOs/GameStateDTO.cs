namespace RPGGame.Network.DTOs;

using System.Collections.Generic;

public class GameStateDTO
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public Dictionary<int, PlayerDTO> Players { get; set; }
    public List<EnemyDTO> Enemies { get; set; }
    public CellDTO[][] Board { get; set; }
    public List<string> LatestLogs { get; set; }
    public bool IsGameOver { get; set; }
    public bool HasPickups { get; set; }
    public bool HasInventoryItems { get; set; }
}