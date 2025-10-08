using UnityEngine;

/// <summary>
/// Main game manager for Pac-Man Motion Game
/// Handles game state, score, and level management
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Game Components")]
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private PelletManager pelletManager;

    [Header("Game State")]
    private int currentLevel = 1;
    private int lives = 3;
    private int score = 0;
    private bool isPaused = false;

    [Header("UI References (to be connected later)")]
    // These will be connected when UI is implemented
    // [SerializeField] private Text scoreText;
    // [SerializeField] private Text livesText;
    // [SerializeField] private Text levelText;

    private static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeGame();
    }

    private void Start()
    {
        // Find components if not assigned
        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<MapGenerator>();
        }

        if (pelletManager == null)
        {
            pelletManager = FindObjectOfType<PelletManager>();
        }

        // Subscribe to events
        if (pelletManager != null)
        {
            pelletManager.OnPelletCollectedEvent += OnPelletCollected;
            pelletManager.OnAllPelletsCollectedEvent += OnLevelComplete;
        }

        StartNewGame();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (pelletManager != null)
        {
            pelletManager.OnPelletCollectedEvent -= OnPelletCollected;
            pelletManager.OnAllPelletsCollectedEvent -= OnLevelComplete;
        }
    }

    private void InitializeGame()
    {
        score = 0;
        lives = 3;
        currentLevel = 1;
        isPaused = false;
    }

    public void StartNewGame()
    {
        InitializeGame();
        LoadLevel(1);
    }

    private void LoadLevel(int levelNumber)
    {
        currentLevel = levelNumber;
        
        if (mapGenerator != null)
        {
            mapGenerator.GenerateMap();
        }

        Debug.Log($"Level {currentLevel} loaded!");
    }

    private void OnPelletCollected(int points, bool isPowerPellet)
    {
        score += points;
        Debug.Log($"Score: {score}");

        if (isPowerPellet)
        {
            OnPowerPelletCollected();
        }

        // Update UI
        UpdateUI();
    }

    private void OnPowerPelletCollected()
    {
        Debug.Log("Power Pellet collected! Ghosts are now vulnerable!");
        // This will be expanded when ghost AI is implemented
    }

    private void OnLevelComplete()
    {
        Debug.Log($"Level {currentLevel} complete!");
        // Load next level
        currentLevel++;
        LoadLevel(currentLevel);
    }

    public void LoseLife()
    {
        lives--;
        Debug.Log($"Lives remaining: {lives}");

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            // Respawn player (to be implemented when player controller is added)
            Debug.Log("Respawning player...");
        }

        UpdateUI();
    }

    private void GameOver()
    {
        Debug.Log($"Game Over! Final Score: {score}");
        isPaused = true;
        // Show game over screen (to be implemented)
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }

    private void UpdateUI()
    {
        // Update UI elements when they are implemented
        // scoreText?.SetText($"Score: {score}");
        // livesText?.SetText($"Lives: {lives}");
        // levelText?.SetText($"Level: {currentLevel}");
    }

    // Public getters
    public int GetScore() { return score; }
    public int GetLives() { return lives; }
    public int GetCurrentLevel() { return currentLevel; }
    public bool IsPaused() { return isPaused; }
}
