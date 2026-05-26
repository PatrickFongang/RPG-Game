namespace RPGGame.Network.DTOs;

using System.Collections.Generic;

public class PlayerDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Health { get; set; }
    public Dictionary<string, int> Wallet { get; set; }
    public ItemDTO? LeftHand { get; set; }
    public ItemDTO? RightHand { get; set; }
    
    public int BaseStrength { get; set; }
    public int BaseAgility { get; set; }
    public int Wisdom { get; set; }
    public int Luck { get; set; }
    public int Aggression { get; set; }
    public string AttackStrategyName { get; set; }
    
    public List<ItemDTO> Backpack { get; set; }
    public int SelectedItemIndex { get; set; }
}