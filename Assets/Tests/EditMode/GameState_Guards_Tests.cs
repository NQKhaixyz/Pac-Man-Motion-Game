using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Tests for GameStateManager transition guards
/// </summary>
public class GameState_Guards_Tests
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
    public void CanTransition_MainMenu_To_Playing_Returns_True()
    {
        // Act & Assert
        Assert.IsTrue(gsm.CanTransition(GameState.MainMenu, GameState.Playing));
    }

    [Test]
    public void CanTransition_Playing_To_Win_Returns_True()
    {
        // Act & Assert
        Assert.IsTrue(gsm.CanTransition(GameState.Playing, GameState.Win));
    }

    [Test]
    public void CanTransition_Playing_To_Lose_Returns_True()
    {
        // Act & Assert
        Assert.IsTrue(gsm.CanTransition(GameState.Playing, GameState.Lose));
    }

    [Test]
    public void CanTransition_Win_To_Replay_Returns_True()
    {
        // Act & Assert
        Assert.IsTrue(gsm.CanTransition(GameState.Win, GameState.Replay));
    }

    [Test]
    public void CanTransition_Lose_To_Replay_Returns_True()
    {
        // Act & Assert
        Assert.IsTrue(gsm.CanTransition(GameState.Lose, GameState.Replay));
    }

    [Test]
    public void CanTransition_Replay_To_Playing_Returns_True()
    {
        // Act & Assert
        Assert.IsTrue(gsm.CanTransition(GameState.Replay, GameState.Playing));
    }

    [Test]
    public void CanTransition_Any_To_MainMenu_Returns_True()
    {
        // Act & Assert
        Assert.IsTrue(gsm.CanTransition(GameState.MainMenu, GameState.MainMenu));
        Assert.IsTrue(gsm.CanTransition(GameState.Playing, GameState.MainMenu));
        Assert.IsTrue(gsm.CanTransition(GameState.Win, GameState.MainMenu));
        Assert.IsTrue(gsm.CanTransition(GameState.Lose, GameState.MainMenu));
        Assert.IsTrue(gsm.CanTransition(GameState.Replay, GameState.MainMenu));
    }

    [Test]
    public void CanTransition_Win_To_Lose_Returns_False()
    {
        // Act & Assert
        Assert.IsFalse(gsm.CanTransition(GameState.Win, GameState.Lose));
    }

    [Test]
    public void CanTransition_Lose_To_Win_Returns_False()
    {
        // Act & Assert
        Assert.IsFalse(gsm.CanTransition(GameState.Lose, GameState.Win));
    }

    [Test]
    public void CanTransition_MainMenu_To_Win_Returns_False()
    {
        // Act & Assert
        Assert.IsFalse(gsm.CanTransition(GameState.MainMenu, GameState.Win));
    }

    [Test]
    public void CanTransition_MainMenu_To_Lose_Returns_False()
    {
        // Act & Assert
        Assert.IsFalse(gsm.CanTransition(GameState.MainMenu, GameState.Lose));
    }

    [Test]
    public void CanTransition_MainMenu_To_Replay_Returns_False()
    {
        // Act & Assert
        Assert.IsFalse(gsm.CanTransition(GameState.MainMenu, GameState.Replay));
    }

    [Test]
    public void CanTransition_Playing_To_Replay_Returns_False()
    {
        // Act & Assert
        Assert.IsFalse(gsm.CanTransition(GameState.Playing, GameState.Replay));
    }

    [Test]
    public void CanTransition_Win_To_Playing_Returns_False()
    {
        // Act & Assert
        Assert.IsFalse(gsm.CanTransition(GameState.Win, GameState.Playing));
    }

    [Test]
    public void CanTransition_Lose_To_Playing_Returns_False()
    {
        // Act & Assert
        Assert.IsFalse(gsm.CanTransition(GameState.Lose, GameState.Playing));
    }

    [Test]
    public void TransitionTo_Rejects_Invalid_Transition_Without_Force()
    {
        // Arrange
        EventBus.EmitRequestStartGame(); // Move to Playing

        // Act
        bool result = gsm.TransitionTo(GameState.Replay, force: false);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(GameState.Playing, gsm.Current);
    }

    [Test]
    public void TransitionTo_Accepts_Invalid_Transition_With_Force()
    {
        // Arrange
        EventBus.EmitRequestStartGame(); // Move to Playing

        // Act
        bool result = gsm.TransitionTo(GameState.Replay, force: true);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(GameState.Replay, gsm.Current);
    }
}
