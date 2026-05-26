namespace RPGGame.Network.DTOs;

using System.Collections.Generic;

public class ItemDTO
{
    public string Name { get; set; }
    public char Symbol { get; set; }
    public string Description { get; set; }
    
    public override string ToString() => $"[{Symbol}] {Name}";
}