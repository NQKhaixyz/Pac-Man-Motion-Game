using NUnit.Framework;
using System;
using UnityEngine;

/// <summary>
/// Tests for EventBus system
/// </summary>
public class EventBus_Tests
{
    [SetUp]
    public void SetUp()
    {
        // Clear all event subscriptions before each test
        EventBus.ClearAllEvents();
    }

    [Test]
    public void EmitSafe_WithNoSubscribers_DoesNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => EventBus.EmitSafe(null));
    }

    [Test]
    public void EmitSafe_WithSubscriber_InvokesEvent()
    {
        // Arrange
        bool eventInvoked = false;
        Action testEvent = () => eventInvoked = true;

        // Act
        EventBus.EmitSafe(testEvent);

        // Assert
        Assert.IsTrue(eventInvoked);
    }

    [Test]
    public void EmitSafe_WithExceptionInHandler_DoesNotThrow()
    {
        // Arrange
        Action testEvent = () => throw new Exception("Test exception");

        // Act & Assert
        Assert.DoesNotThrow(() => EventBus.EmitSafe(testEvent));
    }

    [Test]
    public void RequestStartGame_CanBeSubscribedAndInvoked()
    {
        // Arrange
        bool invoked = false;
        EventBus.RequestStartGame += () => invoked = true;

        // Act
        EventBus.EmitRequestStartGame();

        // Assert
        Assert.IsTrue(invoked);
    }

    [Test]
    public void LevelCompleted_CanBeSubscribedAndInvoked()
    {
        // Arrange
        bool invoked = false;
        EventBus.LevelCompleted += () => invoked = true;

        // Act
        EventBus.EmitLevelCompleted();

        // Assert
        Assert.IsTrue(invoked);
    }

    [Test]
    public void PlayerDead_CanBeSubscribedAndInvoked()
    {
        // Arrange
        bool invoked = false;
        EventBus.PlayerDead += () => invoked = true;

        // Act
        EventBus.EmitPlayerDead();

        // Assert
        Assert.IsTrue(invoked);
    }

    [Test]
    public void RequestReplay_CanBeSubscribedAndInvoked()
    {
        // Arrange
        bool invoked = false;
        EventBus.RequestReplay += () => invoked = true;

        // Act
        EventBus.EmitRequestReplay();

        // Assert
        Assert.IsTrue(invoked);
    }

    [Test]
    public void RequestBackToMenu_CanBeSubscribedAndInvoked()
    {
        // Arrange
        bool invoked = false;
        EventBus.RequestBackToMenu += () => invoked = true;

        // Act
        EventBus.EmitRequestBackToMenu();

        // Assert
        Assert.IsTrue(invoked);
    }

    [Test]
    public void ScoreReset_CanBeSubscribedAndInvoked()
    {
        // Arrange
        bool invoked = false;
        EventBus.ScoreReset += () => invoked = true;

        // Act
        EventBus.EmitScoreReset();

        // Assert
        Assert.IsTrue(invoked);
    }

    [Test]
    public void LevelReset_CanBeSubscribedAndInvoked()
    {
        // Arrange
        bool invoked = false;
        EventBus.LevelReset += () => invoked = true;

        // Act
        EventBus.EmitLevelReset();

        // Assert
        Assert.IsTrue(invoked);
    }

    [Test]
    public void InputLock_CanBeSubscribedAndInvoked()
    {
        // Arrange
        bool invoked = false;
        EventBus.InputLock += () => invoked = true;

        // Act
        EventBus.EmitInputLock();

        // Assert
        Assert.IsTrue(invoked);
    }

    [Test]
    public void InputUnlock_CanBeSubscribedAndInvoked()
    {
        // Arrange
        bool invoked = false;
        EventBus.InputUnlock += () => invoked = true;

        // Act
        EventBus.EmitInputUnlock();

        // Assert
        Assert.IsTrue(invoked);
    }
}
