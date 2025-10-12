using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all pellets in the game, including spawning and tracking collection
/// </summary>
public class PelletManager : MonoBehaviour
{
    [Header("Pellet Prefab")]
    [SerializeField] private GameObject pelletPrefab;

    [Header("Statistics")]
    private int totalPellets = 0;
    private int collectedPellets = 0;
    private int totalScore = 0;

    private List<GameObject> activePellets = new List<GameObject>();

    public delegate void PelletCollectedDelegate(int points, bool isPowerPellet);
    public event PelletCollectedDelegate OnPelletCollectedEvent;

    public delegate void AllPelletsCollectedDelegate();
    public event AllPelletsCollectedDelegate OnAllPelletsCollectedEvent;

    private void Start()
    {
    // Find all existing pellets in the scene (use FindObjectsByType for non-sorted faster option)
    // Find all existing pellets in the scene (include inactive objects).
    // Using Resources.FindObjectsOfTypeAll<T>() is compatible across Unity versions
    // and returns inactive instances as well.
    Pellet[] existingPellets = Resources.FindObjectsOfTypeAll<Pellet>();
        foreach (Pellet pellet in existingPellets)
        {
            activePellets.Add(pellet.gameObject);
        }
        totalPellets = activePellets.Count;
    }

    /// <summary>
    /// Spawns a pellet at the specified position
    /// </summary>
    public GameObject SpawnPellet(Vector3 position, bool isPowerPellet = false)
    {
        GameObject pelletObj;

        if (pelletPrefab != null)
        {
            pelletObj = Instantiate(pelletPrefab, position, Quaternion.identity, transform);
        }
        else
        {
            // Create a new pellet from scratch
            pelletObj = new GameObject("Pellet");
            pelletObj.transform.position = position;
            pelletObj.transform.SetParent(transform);
            pelletObj.AddComponent<Pellet>();
        }

        Pellet pelletComponent = pelletObj.GetComponent<Pellet>();
        if (pelletComponent != null && isPowerPellet)
        {
            pelletComponent.SetAsPowerPellet(true);
        }

        activePellets.Add(pelletObj);
        totalPellets++;

        return pelletObj;
    }

    /// <summary>
    /// Called when a pellet is collected
    /// </summary>
    public void OnPelletCollected(Pellet pellet)
    {
        if (pellet == null) return;

        collectedPellets++;
        int points = pellet.GetPointValue();
        totalScore += points;

        // Remove from active pellets list
        activePellets.Remove(pellet.gameObject);

        // Trigger event
        OnPelletCollectedEvent?.Invoke(points, pellet.IsPowerPellet());

        Debug.Log($"Pellet collected! Points: {points}, Total Score: {totalScore}, Remaining: {GetRemainingPellets()}");

        // Check if all pellets collected
        if (GetRemainingPellets() == 0)
        {
            OnAllPelletsCollected();
        }
    }

    private void OnAllPelletsCollected()
    {
        Debug.Log("All pellets collected! Level complete!");
        OnAllPelletsCollectedEvent?.Invoke();
    }

    /// <summary>
    /// Clears all pellets from the scene
    /// </summary>
    public void ClearAllPellets()
    {
        foreach (GameObject pellet in activePellets)
        {
            if (pellet != null)
            {
                Destroy(pellet);
            }
        }
        activePellets.Clear();
        totalPellets = 0;
        collectedPellets = 0;
    }

    public int GetRemainingPellets()
    {
        return activePellets.Count;
    }

    public int GetTotalScore()
    {
        return totalScore;
    }

    public int GetCollectedPellets()
    {
        return collectedPellets;
    }

    public int GetTotalPellets()
    {
        return totalPellets;
    }
}
