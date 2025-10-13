using UnityEngine;

/// <summary>
/// Example integration showing how to use GameStateManager and EventBus
/// This script demonstrates best practices for working with the game state system
/// </summary>
public class ExampleStateUsage : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameStateManager gameStateManager;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private void Start()
    {
        // Find GameStateManager if not assigned
        if (gameStateManager == null)
        {
            gameStateManager = FindAnyObjectByType<GameStateManager>();
        }

        // Initialize the state system
        if (gameStateManager != null)
        {
            gameStateManager.StartUp();
        }
    }

    private void OnEnable()
    {
        // Subscribe to EventBus events
        EventBus.RequestStartGame += OnRequestStartGame;
        EventBus.LevelCompleted += OnLevelCompleted;
        EventBus.PlayerDead += OnPlayerDead;
        EventBus.RequestReplay += OnRequestReplay;
        EventBus.RequestBackToMenu += OnRequestBackToMenu;

        // Subscribe to system events
        EventBus.ScoreReset += OnScoreReset;
        EventBus.LevelReset += OnLevelReset;
        EventBus.InputLock += OnInputLock;
        EventBus.InputUnlock += OnInputUnlock;

        // Subscribe to state change events
        if (gameStateManager != null)
        {
            gameStateManager.StateWillChange += OnStateWillChange;
            gameStateManager.StateChanged += OnStateChanged;
        }
    }

    private void OnDisable()
    {
        // IMPORTANT: Always unsubscribe to prevent memory leaks
        EventBus.RequestStartGame -= OnRequestStartGame;
        EventBus.LevelCompleted -= OnLevelCompleted;
        EventBus.PlayerDead -= OnPlayerDead;
        EventBus.RequestReplay -= OnRequestReplay;
        EventBus.RequestBackToMenu -= OnRequestBackToMenu;

        EventBus.ScoreReset -= OnScoreReset;
        EventBus.LevelReset -= OnLevelReset;
        EventBus.InputLock -= OnInputLock;
        EventBus.InputUnlock -= OnInputUnlock;

        if (gameStateManager != null)
        {
            gameStateManager.StateWillChange -= OnStateWillChange;
            gameStateManager.StateChanged -= OnStateChanged;
        }
    }

    private void Update()
    {
        // Example: Press keys to trigger events (for testing)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Start game
            EventBus.EmitRequestStartGame();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            // Simulate win
            EventBus.EmitLevelCompleted();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            // Simulate lose
            EventBus.EmitPlayerDead();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Request replay
            EventBus.EmitRequestReplay();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            // Back to menu
            EventBus.EmitRequestBackToMenu();
        }

        // Example: Check current state
        if (gameStateManager != null && Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log($"Current State: {gameStateManager.Current}");
        }
    }

    // Event handlers
    private void OnRequestStartGame()
    {
        if (showDebugLogs) Debug.Log("ExampleStateUsage: Request to start game received");
        // Your game start logic here
    }

    private void OnLevelCompleted()
    {
        if (showDebugLogs) Debug.Log("ExampleStateUsage: Level completed!");
        // Your level complete logic here (e.g., show win screen)
    }

    private void OnPlayerDead()
    {
        if (showDebugLogs) Debug.Log("ExampleStateUsage: Player died!");
        // Your player death logic here (e.g., show game over screen)
    }

    private void OnRequestReplay()
    {
        if (showDebugLogs) Debug.Log("ExampleStateUsage: Replay requested");
        // Your replay setup logic here
    }

    private void OnRequestBackToMenu()
    {
        if (showDebugLogs) Debug.Log("ExampleStateUsage: Back to menu requested");
        // Your menu navigation logic here
    }

    private void OnScoreReset()
    {
        if (showDebugLogs) Debug.Log("ExampleStateUsage: Score reset");
        // Update UI to show score = 0
    }

    private void OnLevelReset()
    {
        if (showDebugLogs) Debug.Log("ExampleStateUsage: Level reset");
        // Update UI to show level = 1
    }

    private void OnInputLock()
    {
        if (showDebugLogs) Debug.Log("ExampleStateUsage: Input locked");
        // Disable player input
    }

    private void OnInputUnlock()
    {
        if (showDebugLogs) Debug.Log("ExampleStateUsage: Input unlocked");
        // Enable player input
    }

    private void OnStateWillChange(GameState from, GameState to)
    {
        if (showDebugLogs) Debug.Log($"ExampleStateUsage: State will change from {from} to {to}");
        // Prepare for state change (e.g., save data, show transition)
    }

    private void OnStateChanged(GameState from, GameState to)
    {
        if (showDebugLogs) Debug.Log($"ExampleStateUsage: State changed from {from} to {to}");
        
        // React to specific state changes
        switch (to)
        {
            case GameState.MainMenu:
                // Show main menu UI
                break;
            case GameState.Playing:
                // Hide menus, show game UI
                break;
            case GameState.Win:
                // Show win screen
                break;
            case GameState.Lose:
                // Show game over screen
                break;
            case GameState.Replay:
                // Show transition/loading
                break;
        }
    }

    // Context menu methods for testing in Unity Editor
    [ContextMenu("Start Game")]
    private void TestStartGame()
    {
        EventBus.EmitRequestStartGame();
    }

    [ContextMenu("Win Level")]
    private void TestWinLevel()
    {
        EventBus.EmitLevelCompleted();
    }

    [ContextMenu("Lose Game")]
    private void TestLoseGame()
    {
        EventBus.EmitPlayerDead();
    }

    [ContextMenu("Request Replay")]
    private void TestReplay()
    {
        EventBus.EmitRequestReplay();
    }

    [ContextMenu("Back to Menu")]
    private void TestBackToMenu()
    {
        EventBus.EmitRequestBackToMenu();
    }

    [ContextMenu("Show Current State")]
    private void ShowCurrentState()
    {
        if (gameStateManager != null)
        {
            Debug.Log($"Current State: {gameStateManager.Current}");
        }
        else
        {
            Debug.LogWarning("GameStateManager not found!");
        }
    }
}
