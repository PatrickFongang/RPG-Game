using RPGGame.Combat;

namespace RPGGame.Items;

public abstract class Item
{
    public string Name { get; protected set; }
    public char Symbol { get; }
    public int Rarity { get; }
    public string Description { get; protected set; }

    public Item(string name, char symbol, int rarity, string description)
    {
        Name = name;
        Symbol = symbol;
        Rarity = rarity;
        Description = description;
    }

    public virtual int GetLuckModifier() => 0;
    public virtual int GetDamageModifier() => 0;
    public virtual int GetWisdomModifier() => 0;

    public virtual bool GoesToBackpack => true;
    public virtual bool IsEquippable => true;
    public virtual bool IsTwoHanded => false;

    public virtual void OnPickedUp(Player player) { }
    public virtual int CalculateDamageWith(IAttackVisitor attack, Player player)
    {
        return attack.CalculateDamage(this, player);
    }

    public virtual int CalculateDefenseWith(IAttackVisitor attack, Player player)
    {
        return attack.CalculateDefense(this, player);
    }

    public override string ToString() => $"[{Symbol}] {Name}";
}