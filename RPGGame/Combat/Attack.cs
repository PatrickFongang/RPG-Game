namespace RPGGame.Combat;
using RPGGame.Items;

public class NormalAttack : IAttackVisitor
{
    public int CalculateDamage(HeavyWeapon weapon, Player player) => weapon.BaseDamage + weapon.GetDamageModifier() + player.Strength + player.Aggression;
    public int CalculateDamage(LightWeapon weapon, Player player) => weapon.BaseDamage + weapon.GetDamageModifier() + player.Agility + player.Luck;
    public int CalculateDamage(MagicWeapon weapon, Player player) => 1;
    public int CalculateDamage(Item notAWeapon, Player player) => 0;

    public int CalculateDefense(HeavyWeapon weapon, Player player) => player.Strength + player.Luck;
    public int CalculateDefense(LightWeapon weapon, Player player) => player.Agility + player.Luck;
    public int CalculateDefense(MagicWeapon weapon, Player player) => player.Agility + player.Luck;
    public int CalculateDefense(Item notAWeapon, Player player) => player.Agility;
}

public class StealthAttack : IAttackVisitor
{
    public int CalculateDamage(HeavyWeapon weapon, Player player) => (weapon.BaseDamage + weapon.GetDamageModifier()) / 2;
    public int CalculateDamage(LightWeapon weapon, Player player) => (weapon.BaseDamage + weapon.GetDamageModifier()) * 2; 
    public int CalculateDamage(MagicWeapon weapon, Player player) => 1;
    public int CalculateDamage(Item notAWeapon, Player player) => 0;

    public int CalculateDefense(HeavyWeapon weapon, Player player) => player.Strength;
    public int CalculateDefense(LightWeapon weapon, Player player) => player.Agility;
    public int CalculateDefense(MagicWeapon weapon, Player player) => 0;
    public int CalculateDefense(Item notAWeapon, Player player) => 0;
}

public class MagicAttack : IAttackVisitor
{
    public int CalculateDamage(HeavyWeapon weapon, Player player) => 1;
    public int CalculateDamage(LightWeapon weapon, Player player) => 1;
    public int CalculateDamage(MagicWeapon weapon, Player player) => weapon.BaseDamage + weapon.GetDamageModifier() + player.Wisdom;
    public int CalculateDamage(Item notAWeapon, Player player) => 0;

    public int CalculateDefense(HeavyWeapon weapon, Player player) => player.Luck;
    public int CalculateDefense(LightWeapon weapon, Player player) => player.Luck;
    public int CalculateDefense(MagicWeapon weapon, Player player) => player.Wisdom * 2;
    public int CalculateDefense(Item notAWeapon, Player player) => player.Luck;
}