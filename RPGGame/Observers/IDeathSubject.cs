namespace RPGGame.Observers;

public interface IDeathSubject
{
    void Attach(IDeathObserver observer);
    void Detach(IDeathObserver observer);
    void NotifyDeath();
}