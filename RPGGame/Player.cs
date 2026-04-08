using RPGGame.Combat;
using RPGGame.Items;

namespace RPGGame;

public class Player(int startX, int startY)
{
    public Inventory Backpack { get; } = new Inventory();

    public Item? LeftHand { get; set; }
    public Item? RightHand { get; set; }
    public int X { get; set; } = startX;
    public int Y { get; set; } = startY;
    public int Coins { get; set; }
    public int Gold { get; set; }

    public int BaseStrength { get; set; } = 8;
    public int BaseAgility { get; set; } = 7;
    public int BaseWisdom { get; set; } = 5;
    public int BaseLuck { get; set; } = 2;

    public int Luck
    {
        get
        {
            int totalLuck = BaseLuck;
            
            if (LeftHand != null) 
            {
                totalLuck += LeftHand.GetLuckModifier();
            }

            if (RightHand != null && RightHand != LeftHand) 
            {
                totalLuck += RightHand.GetLuckModifier();
            }

            return totalLuck;
        }
    }

    public int Strength => BaseStrength; 
    public int Agility => BaseAgility;
    public int Wisdom => BaseWisdom;
    public int Aggression { get; set; } = 10;
    public int Health { get; set; } = 100;
    public IAttackVisitor CurrentAttackStrategy { get; private set; } = new NormalAttack();

    public void ChangeAttackStrategy(IAttackVisitor newStrategy)
    {
        CurrentAttackStrategy = newStrategy;
    }
}