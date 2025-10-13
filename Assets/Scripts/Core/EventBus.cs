using System;
using UnityEngine;

/// <summary>
/// Central event bus for game-wide communication
/// Provides loose coupling between systems via C# events
/// </summary>
public static class EventBus
{
    // Game state transition events
    public static event Action RequestStartGame;
    public static event Action LevelCompleted;
    public static event Action PlayerDead;
    public static event Action RequestReplay;
    public static event Action RequestBackToMenu;

    // Game system events
    public static event Action ScoreReset;
    public static event Action LevelReset;
    public static event Action InputLock;
    public static event Action InputUnlock;

    /// <summary>
    /// Safely invokes an event, catching and logging any exceptions
    /// </summary>
    /// <param name="evt">The event to invoke</param>
    public static void EmitSafe(Action evt)
    {
        if (evt == null) return;

        try
        {
            evt.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    // Helper emit methods for convenience
    public static void EmitRequestStartGame() => EmitSafe(RequestStartGame);
    public static void EmitLevelCompleted() => EmitSafe(LevelCompleted);
    public static void EmitPlayerDead() => EmitSafe(PlayerDead);
    public static void EmitRequestReplay() => EmitSafe(RequestReplay);
    public static void EmitRequestBackToMenu() => EmitSafe(RequestBackToMenu);
    public static void EmitScoreReset() => EmitSafe(ScoreReset);
    public static void EmitLevelReset() => EmitSafe(LevelReset);
    public static void EmitInputLock() => EmitSafe(InputLock);
    public static void EmitInputUnlock() => EmitSafe(InputUnlock);

    /// <summary>
    /// Clears all event subscriptions (useful for testing)
    /// </summary>
    public static void ClearAllEvents()
    {
        RequestStartGame = null;
        LevelCompleted = null;
        PlayerDead = null;
        RequestReplay = null;
        RequestBackToMenu = null;
        ScoreReset = null;
        LevelReset = null;
        InputLock = null;
        InputUnlock = null;
    }
}
