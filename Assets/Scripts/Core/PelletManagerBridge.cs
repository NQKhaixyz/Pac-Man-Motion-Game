using UnityEngine;

/// <summary>
/// Bridges PelletManager events to the EventBus system
/// This component should be attached to the same GameObject as PelletManager
/// </summary>
[RequireComponent(typeof(PelletManager))]
public class PelletManagerBridge : MonoBehaviour
{
    private PelletManager pelletManager;

    private void Awake()
    {
        pelletManager = GetComponent<PelletManager>();
    }

    private void OnEnable()
    {
        if (pelletManager != null)
        {
            pelletManager.OnAllPelletsCollectedEvent += OnAllPelletsCollected;
        }
    }

    private void OnDisable()
    {
        if (pelletManager != null)
        {
            pelletManager.OnAllPelletsCollectedEvent -= OnAllPelletsCollected;
        }
    }

    private void OnAllPelletsCollected()
    {
        Debug.Log("PelletManagerBridge: All pellets collected, emitting LevelCompleted event");
        EventBus.EmitLevelCompleted();
    }
}
