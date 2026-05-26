namespace RPGGame.Network.DTOs;

public class CellDTO
{
    public char Symbol { get; set; }
    public int ForegroundColor { get; set; }
    public ItemDTO TopItemOnGround { get; set; }
}