using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;

/// <summary>
/// Integration tests for PelletManager and EventBus integration
/// </summary>
public class PelletManager_Integration_Tests
{
    private GameObject gameObject;
    private PelletManager pelletManager;
    private PelletManagerBridge bridge;

    [SetUp]
    public void SetUp()
    {
        EventBus.ClearAllEvents();
        gameObject = new GameObject("PelletManager");
        pelletManager = gameObject.AddComponent<PelletManager>();
        bridge = gameObject.AddComponent<PelletManagerBridge>();
    }

    [TearDown]
    public void TearDown()
    {
        if (gameObject != null)
        {
            Object.DestroyImmediate(gameObject);
        }
    }

    [UnityTest]
    public IEnumerator AllPelletsCollected_Triggers_LevelCompleted_Event()
    {
        // Arrange
        bool levelCompletedFired = false;
        EventBus.LevelCompleted += () => levelCompletedFired = true;

        // Create a test pellet
        GameObject pelletObj = new GameObject("TestPellet");
        Pellet pellet = pelletObj.AddComponent<Pellet>();
        
        // Spawn via PelletManager so it's tracked
        pelletManager.SpawnPellet(Vector3.zero);
        
        // Wait for setup
        yield return null;

        // Act - Collect the pellet
        int remainingBefore = pelletManager.GetRemainingPellets();
        pelletManager.OnPelletCollected(pellet);

        // Wait for event propagation
        yield return null;

        // Assert
        Assert.IsTrue(levelCompletedFired, "LevelCompleted event should be triggered when all pellets are collected");
        Assert.AreEqual(0, pelletManager.GetRemainingPellets(), "No pellets should remain");

        // Cleanup
        Object.DestroyImmediate(pelletObj);
    }
}
