namespace RPGGame.Observers;

public interface ISoundSubject
{
    void AttachSoundObserver(ISoundObserver observer);
    void DetachSoundObserver(ISoundObserver observer);
    void NotifySound(int range, Dungeon dungeon);
}