using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;

/// <summary>
/// Full integration tests for GameStateManager with other systems
/// </summary>
public class GameState_Integration_Tests
{
    private GameObject gsmObject;
    private GameStateManager gsm;

    [SetUp]
    public void SetUp()
    {
        EventBus.ClearAllEvents();
        gsmObject = new GameObject("GSM");
        gsm = gsmObject.AddComponent<GameStateManager>();
        gsm.StartUp();
    }

    [TearDown]
    public void TearDown()
    {
        if (gsmObject != null)
        {
            Object.DestroyImmediate(gsmObject);
        }
    }

    [UnityTest]
    public IEnumerator Complete_Game_Flow_MainMenu_To_Win()
    {
        // Arrange
        bool scoreResetFired = false;
        bool levelResetFired = false;
        bool inputUnlockedFired = false;
        bool inputLockedFired = false;

        EventBus.ScoreReset += () => scoreResetFired = true;
        EventBus.LevelReset += () => levelResetFired = true;
        EventBus.InputUnlock += () => inputUnlockedFired = true;
        EventBus.InputLock += () => inputLockedFired = true;

        // Act 1: Start game from MainMenu
        Assert.AreEqual(GameState.MainMenu, gsm.Current);
        EventBus.EmitRequestStartGame();
        yield return null;

        // Assert 1: Should be in Playing state with appropriate events
        Assert.AreEqual(GameState.Playing, gsm.Current);
        Assert.IsTrue(scoreResetFired, "Score should be reset when starting game");
        Assert.IsTrue(levelResetFired, "Level should be reset when starting game");
        Assert.IsTrue(inputUnlockedFired, "Input should be unlocked when starting game");

        // Act 2: Complete level
        EventBus.EmitLevelCompleted();
        yield return null;

        // Assert 2: Should be in Win state with input locked
        Assert.AreEqual(GameState.Win, gsm.Current);
        Assert.IsTrue(inputLockedFired, "Input should be locked when level is completed");
    }

    [UnityTest]
    public IEnumerator Complete_Game_Flow_Playing_To_Lose_To_Replay()
    {
        // Arrange
        EventBus.EmitRequestStartGame();
        yield return null;
        Assert.AreEqual(GameState.Playing, gsm.Current);

        // Act 1: Player dies
        EventBus.EmitPlayerDead();
        yield return null;

        // Assert 1: Should be in Lose state
        Assert.AreEqual(GameState.Lose, gsm.Current);

        // Act 2: Request replay
        bool scoreResetCount = 0;
        bool levelResetCount = 0;
        EventBus.ScoreReset += () => scoreResetCount++;
        EventBus.LevelReset += () => levelResetCount++;
        
        EventBus.EmitRequestReplay();
        yield return null;

        // Assert 2: Should be in Replay state
        Assert.AreEqual(GameState.Replay, gsm.Current);

        // Act 3: Wait for auto-transition
        yield return null;

        // Assert 3: Should auto-transition to Playing
        Assert.AreEqual(GameState.Playing, gsm.Current);
    }

    [UnityTest]
    public IEnumerator BackToMenu_From_Any_State()
    {
        // Test from Playing
        EventBus.EmitRequestStartGame();
        yield return null;
        Assert.AreEqual(GameState.Playing, gsm.Current);

        EventBus.EmitRequestBackToMenu();
        yield return null;
        Assert.AreEqual(GameState.MainMenu, gsm.Current);

        // Test from Win
        EventBus.EmitRequestStartGame();
        yield return null;
        EventBus.EmitLevelCompleted();
        yield return null;
        Assert.AreEqual(GameState.Win, gsm.Current);

        EventBus.EmitRequestBackToMenu();
        yield return null;
        Assert.AreEqual(GameState.MainMenu, gsm.Current);
    }
}
