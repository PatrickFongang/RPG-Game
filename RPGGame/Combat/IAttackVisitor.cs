namespace RPGGame.Combat;
using RPGGame.Items;
public interface IAttackVisitor
{
    int CalculateDamage(HeavyWeapon weapon, Player player);
    int CalculateDamage(LightWeapon weapon, Player player);
    int CalculateDamage(MagicWeapon weapon, Player player);
    int CalculateDamage(Item notAWeapon, Player player); 

    int CalculateDefense(HeavyWeapon weapon, Player player);
    int CalculateDefense(LightWeapon weapon, Player player);
    int CalculateDefense(MagicWeapon weapon, Player player);
    int CalculateDefense(Item notAWeapon, Player player);
}