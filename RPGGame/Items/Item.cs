namespace RPGGame.Items;

public abstract class Item(string name, char symbol, int rarity, String description)
{
    public string Name { get; } = name;
    public char Symbol { get; } = symbol;
    public int Rarity { get; } = rarity;
    public string Description { get; } = description;
    public abstract void OnPickedUp(Player player);
    public abstract void Equip(Player player);

    public override string ToString()
    {
        return $"[{Symbol}] {Name}";
    }
}