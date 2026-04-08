using RPGGame.Combat;

namespace RPGGame.Items;

public class Weapon(string name, char symbol, int rarity, int baseDamage, bool isTwoHanded, string description)
    : Item(name, symbol, rarity, description)
{
    public int BaseDamage { get; protected set; } = baseDamage;
    public override bool IsTwoHanded { get; } = isTwoHanded; 
}
public class HeavyWeapon(string name, char symbol, int rarity, int baseDamage, bool isTwoHanded, string description)
    : Weapon(name, symbol, rarity, baseDamage, isTwoHanded, description)
{ 
    public override int CalculateDamageWith(IAttackVisitor attack, Player player) => attack.CalculateDamage(this, player);
    public override int CalculateDefenseWith(IAttackVisitor attack, Player player) => attack.CalculateDefense(this, player);
}

public class  LightWeapon(string name, char symbol, int rarity, int baseDamage, bool isTwoHanded, string description) 
    : Weapon(name, symbol, rarity, baseDamage, isTwoHanded, description)
{ 
    public override int CalculateDamageWith(IAttackVisitor attack, Player player) => attack.CalculateDamage(this, player);
    public override int CalculateDefenseWith(IAttackVisitor attack, Player player) => attack.CalculateDefense(this, player);
}

public class MagicWeapon(string name, char symbol, int rarity, int baseDamage, bool isTwoHanded, string description) 
    : Weapon(name, symbol, rarity, baseDamage, isTwoHanded, description)
{
    public override int CalculateDamageWith(IAttackVisitor attack, Player player) => attack.CalculateDamage(this, player);
    public override int CalculateDefenseWith(IAttackVisitor attack, Player player) => attack.CalculateDefense(this, player);
}