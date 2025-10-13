using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;

/// <summary>
/// Tests for GameStateManager transitions
/// </summary>
public class GameState_Transition_Tests
{
    private GameStateManager gsm;
    private GameObject gameObject;

    [SetUp]
    public void SetUp()
    {
        EventBus.ClearAllEvents();
        gameObject = new GameObject("GSM");
        gsm = gameObject.AddComponent<GameStateManager>();
        gsm.StartUp(); // Initialize to MainMenu
    }

    [TearDown]
    public void TearDown()
    {
        if (gameObject != null)
        {
            Object.DestroyImmediate(gameObject);
        }
    }

    [Test]
    public void MainMenu_To_Playing_Via_RequestStartGame()
    {
        // Arrange
        bool inputUnlocked = false;
        bool scoreReset = false;
        bool levelReset = false;
        
        EventBus.InputUnlock += () => inputUnlocked = true;
        EventBus.ScoreReset += () => scoreReset = true;
        EventBus.LevelReset += () => levelReset = true;

        // Act
        EventBus.EmitRequestStartGame();

        // Assert
        Assert.AreEqual(GameState.Playing, gsm.Current);
        Assert.IsTrue(inputUnlocked, "InputUnlock should be emitted");
        Assert.IsTrue(scoreReset, "ScoreReset should be emitted");
        Assert.IsTrue(levelReset, "LevelReset should be emitted");
    }

    [Test]
    public void Playing_To_Win_Via_LevelCompleted()
    {
        // Arrange
        EventBus.EmitRequestStartGame(); // Move to Playing
        bool inputLocked = false;
        EventBus.InputLock += () => inputLocked = true;

        // Act
        EventBus.EmitLevelCompleted();

        // Assert
        Assert.AreEqual(GameState.Win, gsm.Current);
        Assert.IsTrue(inputLocked, "InputLock should be emitted");
    }

    [Test]
    public void Playing_To_Lose_Via_PlayerDead()
    {
        // Arrange
        EventBus.EmitRequestStartGame(); // Move to Playing
        bool inputLocked = false;
        EventBus.InputLock += () => inputLocked = true;

        // Act
        EventBus.EmitPlayerDead();

        // Assert
        Assert.AreEqual(GameState.Lose, gsm.Current);
        Assert.IsTrue(inputLocked, "InputLock should be emitted");
    }

    [Test]
    public void Win_To_Replay_Via_RequestReplay()
    {
        // Arrange
        EventBus.EmitRequestStartGame(); // Move to Playing
        EventBus.EmitLevelCompleted();   // Move to Win

        // Act
        EventBus.EmitRequestReplay();

        // Assert
        Assert.AreEqual(GameState.Replay, gsm.Current);
    }

    [Test]
    public void Lose_To_Replay_Via_RequestReplay()
    {
        // Arrange
        EventBus.EmitRequestStartGame(); // Move to Playing
        EventBus.EmitPlayerDead();       // Move to Lose

        // Act
        EventBus.EmitRequestReplay();

        // Assert
        Assert.AreEqual(GameState.Replay, gsm.Current);
    }

    [UnityTest]
    public IEnumerator Replay_Auto_Transitions_To_Playing()
    {
        // Arrange
        EventBus.EmitRequestStartGame(); // Move to Playing
        EventBus.EmitLevelCompleted();   // Move to Win
        EventBus.EmitRequestReplay();    // Move to Replay

        // Act - wait for one frame for auto-transition
        yield return null;

        // Assert
        Assert.AreEqual(GameState.Playing, gsm.Current);
    }

    [Test]
    public void Any_To_MainMenu_Via_RequestBackToMenu()
    {
        // Arrange
        EventBus.EmitRequestStartGame(); // Move to Playing

        // Act
        EventBus.EmitRequestBackToMenu();

        // Assert
        Assert.AreEqual(GameState.MainMenu, gsm.Current);
    }

    [Test]
    public void TransitionTo_Returns_True_On_Success()
    {
        // Act
        bool result = gsm.TransitionTo(GameState.Playing);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(GameState.Playing, gsm.Current);
    }

    [Test]
    public void TransitionTo_Returns_False_On_Invalid_Transition()
    {
        // Arrange
        EventBus.EmitRequestStartGame(); // Move to Playing

        // Act - try to go directly from Playing to Replay (invalid)
        bool result = gsm.TransitionTo(GameState.Replay);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(GameState.Playing, gsm.Current); // Should remain in Playing
    }
}
