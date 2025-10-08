# Pac-Man Motion Game - Technical Documentation

## Architecture Overview

This document provides technical details about the implementation of the Pac-Man map and pellet system (Phase 1 - Issue #01).

## Core Components

### 1. MapGenerator.cs

**Purpose**: Generates the classic Pac-Man maze with walls and pathways.

**Key Features**:
- 28x31 grid layout matching the original Pac-Man arcade game
- Procedural wall generation
- Grid-to-world coordinate conversion
- Walkability checking for pathfinding

**Map Layout Values**:
- `0`: Walkable path (with normal pellet)
- `1`: Wall (impassable)
- `2`: Power pellet location
- `3`: Ghost house (no pellet, reserved area)

**Key Methods**:
```csharp
void GenerateMap()                          // Generates the complete map
void CreateWall(Vector3 position)           // Creates a single wall tile
void CreatePellet(Vector3 position, bool)   // Creates a pellet at position
Vector3 GridToWorldPosition(int x, int y)   // Converts grid to world coords
bool IsWalkable(int x, int y)               // Checks if position is walkable
void ClearMap()                             // Removes all map elements
```

**Configuration**:
- `mapWidth`: 28 cells (default)
- `mapHeight`: 31 cells (default)
- `cellSize`: 1 unit (adjustable)
- `wallColor`: Blue (customizable)

### 2. Pellet.cs

**Purpose**: Individual pellet behavior and collision detection.

**Types**:
1. **Normal Pellet**
   - Point value: 10
   - Size: 0.2 units
   - Color: White

2. **Power Pellet**
   - Point value: 50
   - Size: 0.5 units
   - Color: Yellow
   - Enables ghost vulnerability (when ghost system is implemented)

**Key Methods**:
```csharp
void SetAsPowerPellet(bool isPower)    // Converts to power pellet
int GetPointValue()                     // Returns point value
bool IsPowerPellet()                    // Checks if power pellet
void CollectPellet(GameObject player)   // Handles collection
```

**Collision Detection**:
- Uses Unity 2D CircleCollider2D with trigger enabled
- Detects collision with "Player" tag
- Automatically notifies PelletManager on collection

### 3. PelletManager.cs

**Purpose**: Centralized management of all pellets in the game.

**Responsibilities**:
- Spawn and track all pellets
- Count collected vs remaining pellets
- Calculate total score
- Emit events for game state changes

**Events**:
```csharp
OnPelletCollectedEvent(int points, bool isPowerPellet)  // Fired when pellet collected
OnAllPelletsCollectedEvent()                             // Fired when level complete
```

**Key Methods**:
```csharp
GameObject SpawnPellet(Vector3 pos, bool isPower)  // Creates new pellet
void OnPelletCollected(Pellet pellet)              // Called by Pellet on collect
void ClearAllPellets()                             // Removes all pellets
int GetRemainingPellets()                          // Returns count of remaining
int GetTotalScore()                                // Returns accumulated score
```

**Statistics Tracking**:
- Total pellets spawned
- Pellets collected
- Total score accumulated
- Active pellets list

### 4. GameManager.cs

**Purpose**: High-level game state management and coordination.

**Responsibilities**:
- Game state (score, lives, level)
- Level loading and progression
- Event coordination between systems
- Singleton pattern for global access

**Key Methods**:
```csharp
void StartNewGame()           // Initializes a new game
void LoadLevel(int level)     // Loads specific level
void LoseLife()               // Decrements life counter
void PauseGame()              // Pauses game
void ResumeGame()             // Resumes game
```

**Game State**:
- Current level number
- Lives remaining (default: 3)
- Total score
- Pause state

## Data Flow

### Pellet Collection Flow

```
1. Player collides with Pellet
   └─> Pellet.OnTriggerEnter2D()
       └─> Pellet.CollectPellet()
           └─> PelletManager.OnPelletCollected()
               ├─> Update statistics
               ├─> Fire OnPelletCollectedEvent
               └─> Check if all pellets collected
                   └─> Fire OnAllPelletsCollectedEvent
                       └─> GameManager.OnLevelComplete()
```

### Level Initialization Flow

```
1. GameManager.StartNewGame()
   └─> GameManager.LoadLevel(1)
       └─> MapGenerator.GenerateMap()
           ├─> Create walls from map layout
           └─> Create pellets via PelletManager
               └─> PelletManager.SpawnPellet()
```

## Unity Scene Setup

### Required GameObjects

**GameManager** (Empty GameObject):
- GameManager.cs
- MapGenerator.cs
- PelletManager.cs

**Main Camera** (Camera):
- Orthographic mode
- Size: 15 (to view entire map)
- Background: Black

### Tags Configuration

Required tags in `ProjectSettings/TagManager.asset`:
- `Player`: For Pac-Man character
- `Wall`: For maze walls
- `Pellet`: For normal pellets
- `PowerPellet`: For power pellets
- `Ghost`: For ghost enemies

### Layers

Recommended layer setup:
- Layer 8: Player
- Layer 10: Wall
- Layer 11: Pellet
- Layer 12: Ghost

## Extending the System

### Adding Custom Map Layouts

Modify the `mapLayout` array in `MapGenerator.cs`:

```csharp
private int[,] mapLayout = new int[,]
{
    {1,1,1,1,1}, // Row 0: all walls
    {1,0,0,0,1}, // Row 1: walls with path
    {1,2,0,2,1}, // Row 2: power pellets at corners
    {1,0,0,0,1},
    {1,1,1,1,1}
};
```

### Creating Custom Pellet Types

Extend `Pellet.cs` to add new pellet behaviors:

```csharp
public class FruitPellet : Pellet
{
    [SerializeField] private int bonusPoints = 100;
    
    // Override collection behavior
    protected override void CollectPellet(GameObject player)
    {
        // Custom collection logic
        base.CollectPellet(player);
    }
}
```

### Listening to Game Events

Subscribe to events in your custom scripts:

```csharp
void Start()
{
    PelletManager pm = FindObjectOfType<PelletManager>();
    pm.OnPelletCollectedEvent += HandlePelletCollected;
}

void HandlePelletCollected(int points, bool isPowerPellet)
{
    // Custom logic when pellet is collected
    Debug.Log($"Collected {points} points!");
}
```

## Performance Considerations

### Current Optimizations

1. **Object Pooling**: Not yet implemented (planned for future)
   - Currently creates/destroys pellets on collection
   - Will implement pooling for better performance

2. **Static Batching**: Walls are static and can be batched
   - Enable static batching in Player Settings
   - Mark wall GameObjects as static

3. **Collision Layers**: Use Physics2D layer collision matrix
   - Disable unnecessary layer collisions
   - Configure in ProjectSettings/DynamicsManager

### Future Optimizations

- Implement object pooling for pellets
- Use sprite atlas for better rendering
- Consider ECS (Entity Component System) for massive performance gains
- Implement spatial partitioning for collision detection

## Testing

### Manual Testing Checklist

- [ ] Map generates correctly on scene load
- [ ] All walls are properly positioned
- [ ] Pellets spawn in correct locations
- [ ] Power pellets are larger and different color
- [ ] Score increases when collecting pellets
- [ ] All pellets can be collected
- [ ] Level complete event fires when all pellets collected

### Future Automated Tests

When Unity Test Framework is integrated:
- Unit tests for MapGenerator grid conversion
- Unit tests for PelletManager counting logic
- Integration tests for collection flow
- Scene tests for complete game loop

## Known Limitations

1. **No Player Character**: Player controller not yet implemented
   - Pellet collection will work once player is added with "Player" tag

2. **No Visual Feedback**: Basic sprites only
   - Planned: animations, particle effects, sprite sheets

3. **Single Level**: Only one map layout currently
   - Planned: multiple levels with increasing difficulty

4. **No Ghost AI**: Ghost house exists but no ghosts yet
   - Next phase will implement ghost behavior

## Integration Points for Future Features

### For Player Controller (P1-02)
- Tag player GameObject with "Player"
- Ensure player has Collider2D component
- Use `MapGenerator.IsWalkable()` for movement validation

### For Pose Detection (P1-03, P1-04)
- Input will flow through InputManager
- Convert pose data to directional input
- Feed into player movement system

### For UI System (P1-06)
- Subscribe to `PelletManager.OnPelletCollectedEvent`
- Display `GameManager.GetScore()` in UI
- Show `GameManager.GetLives()` as life icons

### For Sound System (P1-07)
- Subscribe to pellet collection events
- Play "waka waka" sound on normal pellet
- Play power-up sound on power pellet

## References

- [Unity Documentation](https://docs.unity3d.com/)
- [Original Pac-Man Game Design](https://en.wikipedia.org/wiki/Pac-Man)
- [Unity 2D Best Practices](https://unity.com/how-to/2d-game-development)

---

**Last Updated**: 2025-01-08
**Phase**: P1-01 (Map and Pellet System)
**Status**: ✅ Complete
