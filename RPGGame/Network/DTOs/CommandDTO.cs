namespace RPGGame.Network.DTOs;

public class CommandDTO
{
    public int PlayerId { get; set; }
    public string CommandType { get; set; }
    public string Payload { get; set; }
}