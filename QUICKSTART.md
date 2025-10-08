# Quick Start Guide - Pac-Man Motion Game

## For Developers

### Prerequisites
- Unity 2021.3.0f1 or higher
- Basic understanding of Unity and C#
- Git for version control

### Opening the Project

1. **Clone the repository**
   ```bash
   git clone https://github.com/NQKhaixyz/Pac-Man-Motion-Game.git
   cd Pac-Man-Motion-Game
   ```

2. **Open in Unity Hub**
   - Launch Unity Hub
   - Click "Add" → "Add project from disk"
   - Navigate to the cloned folder and select it
   - Click "Open"

3. **Open the main scene**
   - In Unity, navigate to `Assets/Scenes/`
   - Double-click `GameScene.unity`

### Understanding the Scene

When you open `GameScene.unity`, you'll see:

#### Main Camera
- Positioned at (0, 0, -10)
- Orthographic view with size 15
- Black background to match classic Pac-Man

#### GameManager GameObject
This single GameObject contains all the core game systems:

1. **GameManager Component**
   - Manages game state (score, lives, level)
   - Singleton pattern for global access
   - Coordinates between other systems

2. **MapGenerator Component**
   - Generates the 28x31 Pac-Man maze
   - Creates walls and pellets based on layout array
   - Provides grid-to-world conversion utilities

3. **PelletManager Component**
   - Spawns and tracks all pellets
   - Handles pellet collection
   - Fires events for score updates

### Running the Game

1. **Play in Editor**
   - Press the Play button (or F5)
   - The map will automatically generate
   - You'll see the maze with walls and pellets

2. **Current Behavior**
   - Map generates on start
   - Pellets are placed on walkable paths
   - Power pellets appear at corners (yellow, larger)
   - Console logs show system initialization

### What Works Now (P1-01)

✅ **Map Generation**
- Classic Pac-Man maze layout
- 28 columns × 31 rows
- Blue walls
- Proper spacing and alignment

✅ **Pellet System**
- Normal pellets (white, small) - 10 points
- Power pellets (yellow, large) - 50 points
- Collision detection ready
- Score tracking system

✅ **Game Management**
- Score tracking
- Level system foundation
- Event-driven architecture

### What Doesn't Work Yet

❌ **No Player Character**
- Pac-Man sprite and controller not implemented
- Coming in P1-02

❌ **Can't Collect Pellets**
- Need player character first
- Collision system is ready, just needs player

❌ **No Movement**
- Camera and pose detection not implemented
- Coming in P1-03, P1-04

❌ **No Ghosts**
- Ghost AI and sprites coming later
- Ghost house is reserved in the map

## Code Structure

### Key Files to Understand

1. **MapGenerator.cs** - Start here!
   ```csharp
   // The map is defined as a 2D array
   private int[,] mapLayout = new int[,] {
       {1,1,1,1},  // 1 = wall
       {1,0,0,1},  // 0 = path with pellet
       {1,2,2,1},  // 2 = power pellet
       {1,1,1,1}
   };
   ```

2. **Pellet.cs** - Individual pellet behavior
   ```csharp
   // When player touches a pellet
   private void OnTriggerEnter2D(Collider2D other)
   {
       if (other.CompareTag("Player"))
       {
           CollectPellet(other.gameObject);
       }
   }
   ```

3. **PelletManager.cs** - Pellet coordination
   ```csharp
   // Subscribe to events
   pelletManager.OnPelletCollectedEvent += (points, isPower) => {
       Debug.Log($"Collected {points} points!");
   };
   ```

### Adding Your Own Features

#### Example: Change Map Size
```csharp
// In MapGenerator.cs
[SerializeField] private int mapWidth = 20;  // Change from 28
[SerializeField] private int mapHeight = 20; // Change from 31
```

#### Example: Custom Pellet Values
```csharp
// In Pellet.cs
[SerializeField] private int pointValue = 25; // Change from 10
```

#### Example: Listen to Game Events
```csharp
void Start()
{
    PelletManager pm = FindObjectOfType<PelletManager>();
    pm.OnPelletCollectedEvent += OnPelletCollected;
    pm.OnAllPelletsCollectedEvent += OnLevelComplete;
}

void OnPelletCollected(int points, bool isPowerPellet)
{
    Debug.Log($"Got {points} points!");
}

void OnLevelComplete()
{
    Debug.Log("Level finished!");
}
```

## Testing Your Changes

### Manual Testing
1. Press Play in Unity Editor
2. Check Console for any errors
3. Verify map generates correctly
4. Count pellets visually

### Using Unity Debug Tools
- **Scene View**: Check object positions
- **Hierarchy**: Verify GameObjects are created
- **Inspector**: Examine component values
- **Console**: Monitor debug logs

## Common Issues

### Map Doesn't Generate
- Check GameManager has MapGenerator component
- Verify Start() method is being called
- Look for errors in Console

### Pellets Not Visible
- Check camera orthographic size (should be ~15)
- Verify PelletManager is attached to GameManager
- Check pellet colors (white on black background)

### Unity Version Mismatch
- Project made with 2021.3.0f1
- Should work with 2021.3.x or 2022.x
- May need to upgrade if using older version

## Next Steps for Development

### Phase 1-02: Player Controller
Will implement:
- Pac-Man sprite and animation
- Basic movement (arrow keys)
- Collision with walls
- Pellet collection on collision

### Phase 1-03: Camera Integration
Will implement:
- Camera input capture
- Frame processing
- Integration with pose detection library

### Phase 1-04: Pose Detection
Will implement:
- Body pose recognition
- Movement mapping (pose → direction)
- Real-time input processing

## Resources

- 📖 [Full README](README.md) - Project overview
- 📖 [Technical Documentation](TECHNICAL.md) - Detailed architecture
- 🎮 [Unity Documentation](https://docs.unity3d.com/)
- 🐛 [Report Issues](https://github.com/NQKhaixyz/Pac-Man-Motion-Game/issues)

## Getting Help

- **Check Console Logs**: Unity Console shows helpful error messages
- **Read Comments**: Code has extensive comments
- **Review TECHNICAL.md**: Detailed implementation notes
- **Create an Issue**: If stuck, open a GitHub issue

---

**Happy Coding! 🎮**
