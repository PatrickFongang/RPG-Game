using RPGGame.Items;

namespace RPGGame;

public class Player
{
    public Inventory Backpack { get; }

    public Item? LeftHand { get; set; }
    public Item? RightHand { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Coins { get; set; }
    public int Gold { get; set; }

    public int Strength { get; set; } = 70;
    public int Agility { get; set; } = 60;
    public int Wisdom { get; set; } = 50;
    public int Luck { get; set; } = 10;
    public int Aggression { get; set; } = 100;
    public int Health { get; set; } = 100;
    

    public Player(int startX, int startY)
    {
        X = startX;
        Y = startY;
        Backpack = new Inventory();
    }
}