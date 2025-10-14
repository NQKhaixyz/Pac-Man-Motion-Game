using System;

public static class GameEvents
{
    public static Action OnGameStart;
    public static Action OnGameOver;
    public static Action OnReplay;

    public static Action OnPelletEaten;
    public static Action OnPowerPelletEaten;
    public static Action OnGhostEaten;
    public static Action<bool> OnFrightened; // true=start, false=end

    public static Action OnPause;
    public static Action OnResume;
}
