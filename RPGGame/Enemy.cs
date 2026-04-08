namespace RPGGame;

public class Enemy
{
    public string Name { get; }
    public char Symbol { get; }
    public int Health { get; set; }
    public int Attack { get; }
    public int Armor { get; }

    public Enemy(string name, char symbol, int health, int attack, int armor)
    {
        Name = name;
        Symbol = symbol;
        Health = health;
        Attack = attack;
        Armor = armor;
    }
}