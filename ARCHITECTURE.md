# System Architecture Diagrams

## Component Hierarchy

```
GameManager (GameObject)
├── GameManager.cs (MonoBehaviour)
│   ├── Manages: score, lives, level, game state
│   ├── Subscribes to: PelletManager events
│   └── Controls: level loading, game flow
│
├── MapGenerator.cs (MonoBehaviour)
│   ├── Creates: walls, pellets based on layout
│   ├── Provides: grid utilities (IsWalkable, GridToWorld)
│   └── Uses: PelletManager to spawn pellets
│
└── PelletManager.cs (MonoBehaviour)
    ├── Manages: all pellets in scene
    ├── Tracks: total/collected/remaining counts
    ├── Emits: OnPelletCollected, OnAllPelletsCollected
    └── Calculates: total score

Pellets (Individual GameObjects)
└── Pellet.cs (MonoBehaviour)
    ├── Types: Normal (10pts), Power (50pts)
    ├── Detects: collision with Player tag
    └── Notifies: PelletManager on collection
```

## Data Flow Diagram

### Map Generation Flow

```
Game Start
    │
    ├─> GameManager.StartNewGame()
    │       │
    │       └─> GameManager.LoadLevel(1)
    │               │
    │               └─> MapGenerator.GenerateMap()
    │                       │
    │                       ├─> Loop through mapLayout[y,x]
    │                       │       │
    │                       │       ├─> If value == 1: CreateWall(position)
    │                       │       │       │
    │                       │       │       └─> GameObject.CreatePrimitive(Cube)
    │                       │       │           Add BoxCollider2D
    │                       │       │           Tag as "Wall"
    │                       │       │
    │                       │       ├─> If value == 0: CreatePellet(position, false)
    │                       │       │       │
    │                       │       │       └─> PelletManager.SpawnPellet()
    │                       │       │           Create GameObject
    │                       │       │           Add Pellet component
    │                       │       │           Add CircleCollider2D (trigger)
    │                       │       │           Add SpriteRenderer
    │                       │       │
    │                       │       └─> If value == 2: CreatePellet(position, true)
    │                       │               │
    │                       │               └─> PelletManager.SpawnPellet(isPowerPellet=true)
    │                       │
    │                       └─> CenterMap()
    │                           Adjust transform.position
    │
    └─> Map Generated!
        All walls and pellets visible in scene
```

### Pellet Collection Flow

```
Player Movement (future)
    │
    ├─> Player Collider touches Pellet Collider
    │       │
    │       └─> Pellet.OnTriggerEnter2D(Collider2D other)
    │               │
    │               ├─> Check: other.CompareTag("Player") ?
    │               │       │
    │               │       └─> Yes: CollectPellet(player)
    │               │               │
    │               │               ├─> Get point value (10 or 50)
    │               │               │
    │               │               ├─> Notify: PelletManager.OnPelletCollected(this)
    │               │               │       │
    │               │               │       ├─> PelletManager updates statistics
    │               │               │       │   - collectedPellets++
    │               │               │       │   - totalScore += points
    │               │               │       │   - activePellets.Remove(pellet)
    │               │               │       │
    │               │               │       ├─> Fire Event: OnPelletCollectedEvent
    │               │               │       │       │
    │               │               │       │       └─> GameManager receives event
    │               │               │       │           Updates score
    │               │               │       │           Updates UI (future)
    │               │               │       │
    │               │               │       └─> Check: All pellets collected?
    │               │               │               │
    │               │               │               └─> Yes: Fire OnAllPelletsCollectedEvent
    │               │               │                       │
    │               │               │                       └─> GameManager.OnLevelComplete()
    │               │               │                           Load next level
    │               │               │
    │               │               └─> Destroy pellet GameObject
    │               │
    │               └─> No: Ignore collision
    │
    └─> Player continues moving
```

## Class Relationships (UML-style)

```
┌──────────────────────────────────┐
│        GameManager               │
│  ────────────────────────────── │
│  - currentLevel: int             │
│  - lives: int                    │
│  - score: int                    │
│  - isPaused: bool                │
│  ────────────────────────────── │
│  + StartNewGame(): void          │
│  + LoadLevel(int): void          │
│  + LoseLife(): void              │
│  + PauseGame(): void             │
│  + GetScore(): int               │
└────────────┬─────────────────────┘
             │ has reference to
             ├──────────────────┬──────────────────┐
             ▼                  ▼                  ▼
┌────────────────────┐  ┌──────────────────┐  ┌──────────────────┐
│   MapGenerator     │  │  PelletManager   │  │   [Future]       │
│  ────────────────  │  │  ──────────────  │  │   PlayerController│
│  - mapWidth: int   │  │  - totalPellets  │  │   UIManager      │
│  - mapHeight: int  │  │  - collected     │  │   GhostAI        │
│  - cellSize: float │  │  - totalScore    │  └──────────────────┘
│  - mapLayout: int[]│  │  - activePellets │
│  ────────────────  │  │  ──────────────  │
│  + GenerateMap()   │  │  + SpawnPellet() │
│  + ClearMap()      │  │  + ClearAll()    │
│  + IsWalkable()    │  │  + GetRemaining()│
│  + GridToWorld()   │  │  + GetScore()    │
└───────┬────────────┘  └────────┬─────────┘
        │                        │
        │ creates                │ manages
        │                        │
        ▼                        ▼
┌──────────────┐         ┌──────────────┐
│    Wall      │         │    Pellet    │
│  (GameObject)│         │  ──────────  │
│              │         │  - points    │
│  BoxCollider │         │  - isPower   │
│  Tag: "Wall" │         │  ──────────  │
└──────────────┘         │  + Collect() │
                         │  CircleCollider│
                         │  Tag: "Pellet"│
                         └──────────────┘
```

## Event System Flow

```
                    PelletManager (Event Publisher)
                            │
                            │ emits events
            ┌───────────────┼───────────────┐
            │               │               │
            ▼               ▼               ▼
    OnPelletCollectedEvent  │    OnAllPelletsCollectedEvent
    (int, bool)             │           (void)
            │               │               │
            │               │               │
    ┌───────┴────────┐      │      ┌────────┴─────────┐
    │                │      │      │                  │
    ▼                ▼      ▼      ▼                  ▼
GameManager      UIManager  │  GameManager      LevelLoader
UpdateScore()    UpdateUI() │  OnLevelComplete() LoadNext()
                             │
                    [Other Subscribers]
                    SoundManager.PlaySound()
                    ParticleSystem.Emit()
                    Achievement.Check()
```

## Map Layout Structure

```
Legend:
  █ = Wall (value: 1)
  · = Pellet path (value: 0)
  ● = Power pellet (value: 2)
  ░ = Ghost house (value: 3)

Actual 28x31 Map (simplified view):

█████████████████████████████
█·············██·············█
█·████·█████·██·█████·████·█
█●████·█████·██·█████·████●█
█·████·█████·██·█████·████·█
█···························█
█·████·██·████████·██·████·█
█·████·██·████████·██·████·█
█······██····██····██······█
██████·█████·██·█████·██████
██████·█████·██·█████·██████
██████·██··········██·██████
██████·██·███░░███·██·██████
██████·██·█░░░░░░█·██·██████
··········█░░░░░░█··········  <- Tunnel
██████·██·█░░░░░░█·██·██████
██████·██·████████·██·██████
██████·██··········██·██████
██████·██·████████·██·██████
██████·██·████████·██·██████
█·············██·············█
█·████·█████·██·█████·████·█
█·████·█████·██·█████·████·█
█●··██·······██·······██··●█
███·██·██·████████·██·██·███
███·██·██·████████·██·██·███
█······██····██....██......█
█·████████·████·█████████·█
█·████████·████·█████████·█
█···························█
█████████████████████████████

Total: 244 normal pellets + 4 power pellets = 248 pellets
Score: 244×10 + 4×50 = 2,440 + 200 = 2,640 points
```

## Future Integration Points

```
                    [Current System]
                          │
    ┌─────────────────────┼─────────────────────┐
    │                     │                     │
    ▼                     ▼                     ▼
MapGenerator       PelletManager         GameManager
    │                     │                     │
    │                     │                     │
    └─────────────────────┴─────────────────────┘
                          │
            ┌─────────────┼─────────────┐
            │             │             │
            ▼             ▼             ▼
    [Phase 2: P1-02]  [Phase 3]    [Phase 4]
    PlayerController  GhostAI      UISystem
         │                │             │
         ├─Movement       ├─Chase       ├─ScoreDisplay
         ├─Animation      ├─Scatter     ├─LivesDisplay
         └─Collision      ├─Frightened  └─MenuSystem
                          └─Respawn
                              │
                    ┌─────────┴─────────┐
                    │                   │
                    ▼                   ▼
            [Phase 5: P1-03]    [Phase 6: P1-07]
            CameraInput         AudioManager
                 │                   │
                 ├─Capture           ├─Music
                 └─Process           ├─SFX
                     │               └─Voice
                     ▼
            [Phase 7: P1-04]
            PoseDetection
                 │
                 ├─MediaPipe
                 ├─TensorFlow
                 └─MoveNet
```

## File Dependencies

```
GameScene.unity
    │
    └─> References GameManager GameObject
            │
            ├─> GameManager.cs
            │       │
            │       ├─> Depends on: MapGenerator.cs
            │       └─> Depends on: PelletManager.cs
            │
            ├─> MapGenerator.cs
            │       │
            │       └─> Depends on: PelletManager.cs
            │
            ├─> PelletManager.cs
            │       │
            │       └─> Depends on: Pellet.cs (for type reference)
            │
            └─> Creates at runtime:
                    │
                    ├─> Wall GameObjects (multiple)
                    │       └─> Tag: "Wall"
                    │           Component: BoxCollider2D
                    │
                    └─> Pellet GameObjects (multiple)
                            │
                            └─> Component: Pellet.cs
                                Component: CircleCollider2D
                                Component: SpriteRenderer
```

---

## Legend for Diagrams

- `│ ├ └ ─ ┌ ┐` : Connection lines
- `▼ ▶` : Data/control flow direction
- `█` : Wall tile
- `·` : Path with pellet
- `●` : Power pellet
- `░` : Ghost house
- `[Future]` : Components to be implemented
- `(GameObject)` : Unity GameObject
- `(MonoBehaviour)` : Unity component script

---

**Note**: These diagrams represent Phase 1 (P1-01) implementation.
Future phases will expand upon this foundation.
