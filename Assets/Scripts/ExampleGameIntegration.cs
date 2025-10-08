using UnityEngine;

/// <summary>
/// Example script showing how to integrate with the Pac-Man game systems.
/// This demonstrates how to listen to game events and extend functionality.
/// 
/// Usage:
/// 1. Attach this script to any GameObject in the scene
/// 2. Run the game and watch the console for event logs
/// 3. Use this as a template for your own custom features
/// </summary>
public class ExampleGameIntegration : MonoBehaviour
{
    [Header("References")]
    private PelletManager pelletManager;
    private GameManager gameManager;
    private MapGenerator mapGenerator;

    [Header("Debug Options")]
    [SerializeField] private bool showDebugLogs = true;

    private void Start()
    {
        // Find the game systems (they're all on the GameManager GameObject)
        FindGameSystems();

        // Subscribe to game events
        SubscribeToEvents();

        // Example: Get map information
        if (showDebugLogs && mapGenerator != null)
        {
            Debug.Log($"Map Size: {mapGenerator.GetMapWidth()} x {mapGenerator.GetMapHeight()}");
            Debug.Log($"Cell Size: {mapGenerator.GetCellSize()}");
        }
    }

    private void FindGameSystems()
    {
        pelletManager = FindObjectOfType<PelletManager>();
        gameManager = FindObjectOfType<GameManager>();
        mapGenerator = FindObjectOfType<MapGenerator>();

        if (pelletManager == null)
        {
            Debug.LogWarning("PelletManager not found in scene!");
        }
        if (gameManager == null)
        {
            Debug.LogWarning("GameManager not found in scene!");
        }
        if (mapGenerator == null)
        {
            Debug.LogWarning("MapGenerator not found in scene!");
        }
    }

    private void SubscribeToEvents()
    {
        if (pelletManager != null)
        {
            // Subscribe to pellet collection events
            pelletManager.OnPelletCollectedEvent += HandlePelletCollected;
            pelletManager.OnAllPelletsCollectedEvent += HandleLevelComplete;
        }
    }

    private void OnDestroy()
    {
        // IMPORTANT: Always unsubscribe from events to prevent memory leaks
        if (pelletManager != null)
        {
            pelletManager.OnPelletCollectedEvent -= HandlePelletCollected;
            pelletManager.OnAllPelletsCollectedEvent -= HandleLevelComplete;
        }
    }

    /// <summary>
    /// Called whenever a pellet is collected
    /// </summary>
    private void HandlePelletCollected(int points, bool isPowerPellet)
    {
        if (showDebugLogs)
        {
            string pelletType = isPowerPellet ? "POWER PELLET" : "normal pellet";
            Debug.Log($"[Example] Collected {pelletType}! Points: {points}");
            Debug.Log($"[Example] Total Score: {pelletManager.GetTotalScore()}");
            Debug.Log($"[Example] Remaining Pellets: {pelletManager.GetRemainingPellets()}");
        }

        // Example: Add custom behavior when power pellet is collected
        if (isPowerPellet)
        {
            OnPowerPelletCollected();
        }
    }

    /// <summary>
    /// Called when all pellets are collected
    /// </summary>
    private void HandleLevelComplete()
    {
        if (showDebugLogs)
        {
            Debug.Log($"[Example] LEVEL COMPLETE!");
            Debug.Log($"[Example] Final Score: {pelletManager.GetTotalScore()}");
            Debug.Log($"[Example] Current Level: {gameManager.GetCurrentLevel()}");
        }

        // Example: Add custom celebration effects here
        // PlayLevelCompleteAnimation();
        // ShowVictoryScreen();
    }

    /// <summary>
    /// Custom behavior when power pellet is collected
    /// </summary>
    private void OnPowerPelletCollected()
    {
        if (showDebugLogs)
        {
            Debug.Log("[Example] Power mode activated! (Ghost chase mode will be implemented later)");
        }

        // Example: Add visual/audio feedback
        // PlayPowerUpSound();
        // ActivateScreenFlash();
        // EnableGhostVulnerability();
    }

    // Example: Utility method to check if a position is walkable
    private void ExampleCheckWalkable()
    {
        if (mapGenerator != null)
        {
            // Check if grid position (5, 5) is walkable
            bool isWalkable = mapGenerator.IsWalkable(5, 5);
            Debug.Log($"Position (5,5) is walkable: {isWalkable}");

            // Get world position from grid coordinates
            Vector3 worldPos = mapGenerator.GridToWorldPosition(5, 5);
            Debug.Log($"Grid (5,5) in world coordinates: {worldPos}");
        }
    }

    // Example: Manual test methods (call these from other scripts or debug buttons)
    [ContextMenu("Test: Get Current Stats")]
    public void TestGetCurrentStats()
    {
        if (pelletManager != null)
        {
            Debug.Log("=== CURRENT GAME STATS ===");
            Debug.Log($"Total Pellets: {pelletManager.GetTotalPellets()}");
            Debug.Log($"Collected: {pelletManager.GetCollectedPellets()}");
            Debug.Log($"Remaining: {pelletManager.GetRemainingPellets()}");
            Debug.Log($"Total Score: {pelletManager.GetTotalScore()}");
        }

        if (gameManager != null)
        {
            Debug.Log($"Current Level: {gameManager.GetCurrentLevel()}");
            Debug.Log($"Lives: {gameManager.GetLives()}");
            Debug.Log($"Is Paused: {gameManager.IsPaused()}");
        }
    }

    [ContextMenu("Test: Check Grid Position Walkable")]
    public void TestCheckWalkable()
    {
        ExampleCheckWalkable();
    }

    // Example: How to spawn a custom pellet programmatically
    [ContextMenu("Test: Spawn Custom Pellet")]
    public void TestSpawnCustomPellet()
    {
        if (pelletManager != null)
        {
            // Spawn a power pellet at world position (0, 0, 0)
            Vector3 spawnPos = Vector3.zero;
            GameObject pellet = pelletManager.SpawnPellet(spawnPos, isPowerPellet: true);
            Debug.Log($"[Example] Spawned custom pellet at {spawnPos}");
        }
    }

    // Example: Demonstrate how to access game state
    private void Update()
    {
        // Example: Press 'I' key to show game info
        if (Input.GetKeyDown(KeyCode.I))
        {
            TestGetCurrentStats();
        }

        // Example: Press 'W' key to test walkability
        if (Input.GetKeyDown(KeyCode.W))
        {
            TestCheckWalkable();
        }

        // Example: Press 'S' key to spawn a pellet
        if (Input.GetKeyDown(KeyCode.S))
        {
            TestSpawnCustomPellet();
        }
    }
}

/*
 * HOW TO USE THIS EXAMPLE:
 * 
 * 1. Create a new empty GameObject in your scene
 * 2. Attach this script to it
 * 3. Run the game
 * 4. Press keys to test:
 *    - I: Show current game stats
 *    - W: Test walkability checking
 *    - S: Spawn a custom pellet
 * 
 * EXTENDING THIS:
 * 
 * - Add your own event handlers
 * - Create custom UI elements that display stats
 * - Implement power-up effects
 * - Add visual feedback for collections
 * - Create achievement systems
 * - Track high scores
 * - Implement combos and multipliers
 * 
 * BEST PRACTICES:
 * 
 * - Always null-check before using references
 * - Subscribe to events in Start() or Awake()
 * - Always unsubscribe in OnDestroy()
 * - Use [SerializeField] for Inspector fields
 * - Add [Header] attributes to organize fields
 * - Add XML comments for documentation
 * - Use ContextMenu for testing methods
 */
