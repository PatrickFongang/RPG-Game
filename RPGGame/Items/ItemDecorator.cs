using RPGGame.Combat;

namespace RPGGame.Items;

public abstract class ItemDecorator : Item
{
    protected readonly Item _wrappee;

    public ItemDecorator(Item wrappee, string prefix) 
        : base($"{wrappee.Name} ({prefix})", wrappee.Symbol, wrappee.Rarity, wrappee.Description)
    {
        _wrappee = wrappee;
    }

    public override int GetLuckModifier() => _wrappee.GetLuckModifier();
    public override int GetDamageModifier() => _wrappee.GetDamageModifier();
    public override int GetWisdomModifier() => _wrappee.GetWisdomModifier();
    
    public override int CalculateDamageWith(IAttackVisitor attack, Player player) => _wrappee.CalculateDamageWith(attack, player);
    public override int CalculateDefenseWith(IAttackVisitor attack, Player player) => _wrappee.CalculateDefenseWith(attack, player);

    public override bool GoesToBackpack => _wrappee.GoesToBackpack;
    public override bool IsEquippable => _wrappee.IsEquippable;
    public override bool IsTwoHanded => _wrappee.IsTwoHanded;

    public override void OnPickedUp(Player player) => _wrappee.OnPickedUp(player);
}

public class LuckModifierDecorator : ItemDecorator
{
    private readonly int _luckChange;

    public LuckModifierDecorator(Item wrappee, string prefix, int luckChange) 
        : base(wrappee, prefix)
    {
        _luckChange = luckChange;
    }

    public override int GetLuckModifier()
    {
        return _wrappee.GetLuckModifier() + _luckChange; 
    }
}

public class DamageModifierDecorator : ItemDecorator
{
    private readonly int _damageChange;

    public DamageModifierDecorator(Item wrappee, string prefix, int damageChange) 
        : base(wrappee, prefix)
    {
        _damageChange = damageChange;
    }

    public override int GetDamageModifier()
    {
        return _wrappee.GetDamageModifier() + _damageChange;
    }
}
public class WisdomModifierDecorator : ItemDecorator
{
    private readonly int _wisdomChange;

    public WisdomModifierDecorator(Item wrappee, string prefix, int wisdomChange) 
        : base(wrappee, prefix)
    {
        _wisdomChange = wisdomChange;
    }

    public override int GetWisdomModifier()
    {
        return _wrappee.GetWisdomModifier() + _wisdomChange;
    }
}