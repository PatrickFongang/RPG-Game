namespace RPGGame.Observers;

public class SpeciesGroup : IDeathSubject
{
    private readonly List<IDeathObserver> _observers = new List<IDeathObserver>();

    public void Attach(IDeathObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void Detach(IDeathObserver observer)
    {
        _observers.Remove(observer);
    }

    public void NotifyDeath()
    {
        foreach (var observer in _observers.ToList())
        {
            observer.OnFellowEnemyDied();
        }
    }
}