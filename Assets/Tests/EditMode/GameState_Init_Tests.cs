using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Tests for GameStateManager initialization
/// </summary>
public class GameState_Init_Tests
{
    private GameStateManager gsm;
    private GameObject gameObject;

    [SetUp]
    public void SetUp()
    {
        EventBus.ClearAllEvents();
        gameObject = new GameObject("GSM");
        gsm = gameObject.AddComponent<GameStateManager>();
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
    public void StartUp_Sets_MainMenu_And_Fires_StateChanged()
    {
        // Arrange
        int changedCount = 0;
        GameState oldState = GameState.MainMenu;
        GameState newState = GameState.MainMenu;
        
        gsm.StateChanged += (from, to) => {
            changedCount++;
            oldState = from;
            newState = to;
        };

        // Act
        gsm.StartUp();

        // Assert
        Assert.AreEqual(GameState.MainMenu, gsm.Current);
        Assert.AreEqual(1, changedCount);
        Assert.AreEqual(GameState.MainMenu, newState);
    }

    [Test]
    public void StartUp_Fires_StateWillChange_Before_StateChanged()
    {
        // Arrange
        int willChangeCount = 0;
        int changedCount = 0;
        bool willChangeFiredFirst = false;

        gsm.StateWillChange += (from, to) => {
            willChangeCount++;
            if (changedCount == 0) willChangeFiredFirst = true;
        };

        gsm.StateChanged += (from, to) => {
            changedCount++;
        };

        // Act
        gsm.StartUp();

        // Assert
        Assert.AreEqual(1, willChangeCount);
        Assert.AreEqual(1, changedCount);
        Assert.IsTrue(willChangeFiredFirst);
    }

    [Test]
    public void Initial_Current_Is_Default_BeforeStartUp()
    {
        // Assert - default value should be first enum value (MainMenu)
        Assert.AreEqual(GameState.MainMenu, gsm.Current);
    }
}
