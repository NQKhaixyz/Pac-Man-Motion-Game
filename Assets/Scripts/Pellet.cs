using UnityEngine;

/// <summary>
/// Represents a single pellet (dot) that Pac-Man can collect
/// </summary>
public class Pellet : MonoBehaviour
{
    [Header("Pellet Settings")]
    [SerializeField] private int pointValue = 10;
    [SerializeField] private bool isPowerPellet = false;
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color powerPelletColor = Color.yellow;
    [SerializeField] private float normalSize = 0.2f;
    [SerializeField] private float powerPelletSize = 0.5f;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D pelletCollider;

    private void Awake()
    {
        InitializePellet();
    }

    private void InitializePellet()
    {
        // Setup sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Setup collider
        pelletCollider = GetComponent<CircleCollider2D>();
        if (pelletCollider == null)
        {
            pelletCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        pelletCollider.isTrigger = true;

        // Apply visual settings based on pellet type
        if (isPowerPellet)
        {
            spriteRenderer.color = powerPelletColor;
            transform.localScale = Vector3.one * powerPelletSize;
            pointValue = 50;
        }
        else
        {
            spriteRenderer.color = normalColor;
            transform.localScale = Vector3.one * normalSize;
        }

        // Create a simple circle sprite if none exists
        CreateCircleSprite();
    }

    private void CreateCircleSprite()
    {
        if (spriteRenderer.sprite == null)
        {
            // Create a simple circle texture
            int resolution = 32;
            Texture2D texture = new Texture2D(resolution, resolution);
            Color[] pixels = new Color[resolution * resolution];
            
            Vector2 center = new Vector2(resolution / 2f, resolution / 2f);
            float radius = resolution / 2f;

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    pixels[y * resolution + x] = distance <= radius ? Color.white : Color.clear;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Point;

            spriteRenderer.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, resolution, resolution),
                new Vector2(0.5f, 0.5f),
                resolution
            );
        }
    }

    public void SetAsPowerPellet(bool isPower)
    {
        isPowerPellet = isPower;
        InitializePellet();
    }

    public int GetPointValue()
    {
        return pointValue;
    }

    public bool IsPowerPellet()
    {
        return isPowerPellet;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if Pac-Man collided with the pellet
        if (other.CompareTag("Player"))
        {
            CollectPellet(other.gameObject);
        }
    }

    private void CollectPellet(GameObject player)
    {
    // Notify the PelletManager about collection
    // Use Object.FindAnyObjectByType<T>() instead of deprecated FindObjectOfType<T>()
    PelletManager pelletManager = Object.FindAnyObjectByType<PelletManager>();
        if (pelletManager != null)
        {
            pelletManager.OnPelletCollected(this);
        }

        // Destroy the pellet
        Destroy(gameObject);
    }
}
