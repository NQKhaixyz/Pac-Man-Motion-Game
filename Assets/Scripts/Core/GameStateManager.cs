using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Game state enumeration
/// </summary>
public enum GameState
{
    MainMenu,
    Playing,
    Win,
    Lose,
    Replay
}

/// <summary>
/// Manages game state transitions and side effects
/// </summary>
public class GameStateManager : MonoBehaviour
{
    /// <summary>
    /// Current game state
    /// </summary>
    public GameState Current { get; private set; } = GameState.MainMenu;

    /// <summary>
    /// Event fired before state transition (can be used for validation/preparation)
    /// </summary>
    public event Action<GameState, GameState> StateWillChange;

    /// <summary>
    /// Event fired after state transition is complete
    /// </summary>
    public event Action<GameState, GameState> StateChanged;

    private Coroutine replayTransitionCoroutine;

    private void OnEnable()
    {
        // Subscribe to EventBus events
        EventBus.RequestStartGame += OnRequestStartGame;
        EventBus.LevelCompleted += OnLevelCompleted;
        EventBus.PlayerDead += OnPlayerDead;
        EventBus.RequestReplay += OnRequestReplay;
        EventBus.RequestBackToMenu += OnRequestBackToMenu;
    }

    private void OnDisable()
    {
        // Unsubscribe from EventBus events
        EventBus.RequestStartGame -= OnRequestStartGame;
        EventBus.LevelCompleted -= OnLevelCompleted;
        EventBus.PlayerDead -= OnPlayerDead;
        EventBus.RequestReplay -= OnRequestReplay;
        EventBus.RequestBackToMenu -= OnRequestBackToMenu;
    }

    /// <summary>
    /// Initializes the game state to MainMenu
    /// </summary>
    public void StartUp()
    {
        TransitionTo(GameState.MainMenu, force: true);
    }

    /// <summary>
    /// Attempts to transition to a new game state
    /// </summary>
    /// <param name="target">Target state</param>
    /// <param name="force">If true, bypasses validation</param>
    /// <returns>True if transition succeeded, false otherwise</returns>
    public bool TransitionTo(GameState target, bool force = false)
    {
        // Check if transition is valid
        if (!force && !CanTransition(Current, target))
        {
            Debug.LogWarning($"Invalid transition from {Current} to {target}");
            return false;
        }

        GameState oldState = Current;

        // Fire StateWillChange event
        StateWillChange?.Invoke(oldState, target);

        // Update current state
        Current = target;

        // Execute side effects based on transition
        ExecuteSideEffects(oldState, target);

        // Fire StateChanged event
        StateChanged?.Invoke(oldState, target);

        Debug.Log($"State transition: {oldState} -> {target}");

        return true;
    }

    /// <summary>
    /// Checks if a state transition is valid
    /// </summary>
    /// <param name="from">Source state</param>
    /// <param name="to">Target state</param>
    /// <returns>True if transition is valid, false otherwise</returns>
    public bool CanTransition(GameState from, GameState to)
    {
        // Any state can transition to MainMenu
        if (to == GameState.MainMenu)
            return true;

        // Define valid transitions
        switch (from)
        {
            case GameState.MainMenu:
                return to == GameState.Playing;

            case GameState.Playing:
                return to == GameState.Win || to == GameState.Lose;

            case GameState.Win:
            case GameState.Lose:
                return to == GameState.Replay;

            case GameState.Replay:
                return to == GameState.Playing;

            default:
                return false;
        }
    }

    /// <summary>
    /// Executes side effects when transitioning between states
    /// </summary>
    private void ExecuteSideEffects(GameState from, GameState to)
    {
        // MainMenu → Playing: Unlock input, reset score and level
        if (from == GameState.MainMenu && to == GameState.Playing)
        {
            EventBus.EmitInputUnlock();
            EventBus.EmitScoreReset();
            EventBus.EmitLevelReset();
        }

        // Playing → Win/Lose: Lock input
        if (from == GameState.Playing && (to == GameState.Win || to == GameState.Lose))
        {
            EventBus.EmitInputLock();
        }

        // Win/Lose → Replay: Reset and schedule auto-transition to Playing
        if ((from == GameState.Win || from == GameState.Lose) && to == GameState.Replay)
        {
            EventBus.EmitScoreReset();
            EventBus.EmitLevelReset();
            
            // Schedule auto-transition to Playing after one frame
            if (replayTransitionCoroutine != null)
            {
                StopCoroutine(replayTransitionCoroutine);
            }
            replayTransitionCoroutine = StartCoroutine(AutoTransitionFromReplay());
        }
    }

    /// <summary>
    /// Automatically transitions from Replay to Playing after one frame
    /// </summary>
    private IEnumerator AutoTransitionFromReplay()
    {
        yield return null; // Wait one frame
        
        if (Current == GameState.Replay)
        {
            TransitionTo(GameState.Playing);
            EventBus.EmitInputUnlock();
        }
    }

    // EventBus event handlers
    private void OnRequestStartGame()
    {
        TransitionTo(GameState.Playing);
    }

    private void OnLevelCompleted()
    {
        TransitionTo(GameState.Win);
    }

    private void OnPlayerDead()
    {
        TransitionTo(GameState.Lose);
    }

    private void OnRequestReplay()
    {
        TransitionTo(GameState.Replay);
    }

    private void OnRequestBackToMenu()
    {
        TransitionTo(GameState.MainMenu);
    }
}
