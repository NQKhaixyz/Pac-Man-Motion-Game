using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates the Pac-Man game map with walls and pellets
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int mapWidth = 28;
    [SerializeField] private int mapHeight = 31;
    [SerializeField] private float cellSize = 1f;

    [Header("Prefabs")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject pelletPrefab;

    [Header("Visual Settings")]
    [SerializeField] private Color wallColor = Color.blue;
    [SerializeField] private float wallWidth = 0.2f;

    private PelletManager pelletManager;
    private List<GameObject> walls = new List<GameObject>();

    // Classic Pac-Man map layout (1 = wall, 0 = path, 2 = power pellet position)
    // Simplified 28x31 grid representation
    private int[,] mapLayout = new int[,]
    {
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1},
        {1,2,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,2,1},
        {1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,0,1},
        {1,0,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,0,1},
        {1,0,0,0,0,0,0,1,1,0,0,0,0,1,1,0,0,0,0,1,1,0,0,0,0,0,0,1},
        {1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1},
        {1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1},
        {1,1,1,1,1,1,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,1,1,1,1,1,1},
        {1,1,1,1,1,1,0,1,1,0,1,1,1,3,3,1,1,1,0,1,1,0,1,1,1,1,1,1},
        {1,1,1,1,1,1,0,1,1,0,1,3,3,3,3,3,3,1,0,1,1,0,1,1,1,1,1,1},
        {0,0,0,0,0,0,0,0,0,0,1,3,3,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,1,1,1,1,0,1,1,0,1,3,3,3,3,3,3,1,0,1,1,0,1,1,1,1,1,1},
        {1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1},
        {1,1,1,1,1,1,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,1,1,1,1,1,1},
        {1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1},
        {1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1},
        {1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1},
        {1,2,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,2,1},
        {1,1,1,0,1,1,0,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,1,1,0,1,1,1},
        {1,1,1,0,1,1,0,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,1,1,0,1,1,1},
        {1,0,0,0,0,0,0,1,1,0,0,0,0,1,1,0,0,0,0,1,1,0,0,0,0,0,0,1},
        {1,0,1,1,1,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,1,1,0,1},
        {1,0,1,1,1,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,1,1,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
    };

    private void Start()
    {
        pelletManager = GetComponent<PelletManager>();
        if (pelletManager == null)
        {
            pelletManager = gameObject.AddComponent<PelletManager>();
        }

        GenerateMap();
    }

    /// <summary>
    /// Generates the complete Pac-Man map
    /// </summary>
    public void GenerateMap()
    {
        ClearMap();

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Vector3 position = new Vector3(x * cellSize, -y * cellSize, 0);
                int cellValue = mapLayout[y, x];

                switch (cellValue)
                {
                    case 1: // Wall
                        CreateWall(position);
                        break;
                    case 0: // Path with pellet
                        CreatePellet(position, false);
                        break;
                    case 2: // Power pellet
                        CreatePellet(position, true);
                        break;
                    case 3: // Ghost house (no pellet)
                        // Empty space, do nothing
                        break;
                }
            }
        }

        CenterMap();
    }

    private void CreateWall(Vector3 position)
    {
        GameObject wall;

        if (wallPrefab != null)
        {
            wall = Instantiate(wallPrefab, position, Quaternion.identity, transform);
        }
        else
        {
            // Create a simple wall cube
            wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.position = position;
            wall.transform.localScale = new Vector3(cellSize, cellSize, wallWidth);
            wall.transform.SetParent(transform);
            wall.name = "Wall";

            // Set color
            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = wallColor;
            }

            // Add box collider
            BoxCollider2D collider = wall.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = wall.AddComponent<BoxCollider2D>();
            }
            collider.size = new Vector2(cellSize, cellSize);
        }

        wall.tag = "Wall";
        walls.Add(wall);
    }

    private void CreatePellet(Vector3 position, bool isPowerPellet)
    {
        if (pelletManager != null)
        {
            pelletManager.SpawnPellet(position, isPowerPellet);
        }
    }

    /// <summary>
    /// Centers the map in the scene
    /// </summary>
    private void CenterMap()
    {
        float offsetX = -(mapWidth * cellSize) / 2f;
        float offsetY = (mapHeight * cellSize) / 2f;
        transform.position = new Vector3(offsetX, offsetY, 0);
    }

    /// <summary>
    /// Clears all generated map elements
    /// </summary>
    public void ClearMap()
    {
        // Clear walls
        foreach (GameObject wall in walls)
        {
            if (wall != null)
            {
                Destroy(wall);
            }
        }
        walls.Clear();

        // Clear pellets
        if (pelletManager != null)
        {
            pelletManager.ClearAllPellets();
        }
    }

    public int GetMapWidth()
    {
        return mapWidth;
    }

    public int GetMapHeight()
    {
        return mapHeight;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    // Helper method to get world position from grid coordinates
    public Vector3 GridToWorldPosition(int x, int y)
    {
        float offsetX = -(mapWidth * cellSize) / 2f;
        float offsetY = (mapHeight * cellSize) / 2f;
        return new Vector3(x * cellSize + offsetX, -y * cellSize + offsetY, 0);
    }

    // Helper method to check if a grid position is walkable
    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
        {
            return false;
        }
        return mapLayout[y, x] != 1; // Not a wall
    }
}
