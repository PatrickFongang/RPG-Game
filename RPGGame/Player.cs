using RPGGame.Combat;
using RPGGame.Config;
using RPGGame.Items;
using RPGGame.Observers;

namespace RPGGame;

public class Player(int startX, int startY) : ISoundSubject
{
    public Inventory Backpack { get; } = new Inventory();
    public Dictionary<string, int> Wallet { get; } = new Dictionary<string, int>();
    public string PlayerName { get; private set; } = ConfigManager.Instance.Config.PlayerName;

    public Item? LeftHand { get; set; }
    public Item? RightHand { get; set; }
    public int X { get; set; } = startX;
    public int Y { get; set; } = startY;

    public int BaseStrength { get; set; } = 8;
    public int BaseAgility { get; set; } = 7;
    public int BaseWisdom { get; set; } = 5;
    public int BaseLuck { get; set; } = 2;

    private readonly List<ISoundObserver> _soundObservers = new List<ISoundObserver>();
    
    public void AttachSoundObserver(ISoundObserver observer)
    {
        if (!_soundObservers.Contains(observer))
        {
            _soundObservers.Add(observer);
        }
    }

    public void DetachSoundObserver(ISoundObserver observer)
    {
        _soundObservers.Remove(observer);
    }

    public void NotifySound(int range, Dungeon dungeon)
    {
        foreach (var observer in _soundObservers.ToList())
        {
            observer.OnSoundHeard(X, Y, range, dungeon);
        }
    }

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
    
    public int Wisdom
    {
        get
        {
            int totalWisdom = BaseWisdom;
            if (LeftHand != null) totalWisdom += LeftHand.GetWisdomModifier();
            if (RightHand != null && RightHand != LeftHand) totalWisdom += RightHand.GetWisdomModifier();
            return totalWisdom;
        }
    }

    public int Strength => BaseStrength; 
    public int Agility => BaseAgility;
    public int Aggression { get; set; } = 10;
    public int Health { get; set; } = 100;
    public IAttackVisitor CurrentAttackStrategy { get; private set; } = new NormalAttack();

    public void ChangeAttackStrategy(IAttackVisitor newStrategy)
    {
        CurrentAttackStrategy = newStrategy;
    }
}