namespace RPGGame.Observers;

public interface ISoundObserver
{
    void OnSoundHeard(int sourceX, int sourceY, int range, Dungeon dungeon);
}