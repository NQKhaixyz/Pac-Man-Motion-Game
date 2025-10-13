# Core Game Systems

This directory contains the core game state management and event systems for Pac-Man Motion Game.

## Components

### EventBus.cs
Central event bus for game-wide communication using C# events and delegates. Provides loose coupling between systems.

**Key Features:**
- Static event declarations for game-wide access
- Safe event invocation with exception handling (`EmitSafe`)
- Helper methods for common events
- Test-friendly with `ClearAllEvents()` method

**Events:**
- `RequestStartGame` - Request to start a new game
- `LevelCompleted` - Level is completed (all pellets collected)
- `PlayerDead` - Player has died
- `RequestReplay` - Request to replay after win/lose
- `RequestBackToMenu` - Request to return to main menu
- `ScoreReset` - Reset the score
- `LevelReset` - Reset the level
- `InputLock` - Lock player input
- `InputUnlock` - Unlock player input

**Usage:**
```csharp
// Subscribe to an event
EventBus.LevelCompleted += OnLevelCompleted;

// Emit an event
EventBus.EmitLevelCompleted();

// Unsubscribe
EventBus.LevelCompleted -= OnLevelCompleted;
```

### GameStateManager.cs
Manages game state transitions and enforces valid state flow.

**Game States:**
- `MainMenu` - Main menu screen
- `Playing` - Active gameplay
- `Win` - Level completed successfully
- `Lose` - Player died
- `Replay` - Transitional state before restarting

**State Transition Rules:**
```
MainMenu → Playing (via RequestStartGame)
Playing → Win (via LevelCompleted)
Playing → Lose (via PlayerDead)
Win/Lose → Replay (via RequestReplay)
Replay → Playing (automatic after 1 frame)
Any → MainMenu (via RequestBackToMenu)
```

**Events:**
- `StateWillChange` - Fired before state transition
- `StateChanged` - Fired after state transition

**Usage:**
```csharp
// Get current state
GameState current = gameStateManager.Current;

// Check if transition is valid
bool canTransition = gameStateManager.CanTransition(GameState.Playing, GameState.Win);

// Force a transition (bypasses validation)
gameStateManager.TransitionTo(GameState.MainMenu, force: true);
```

**Side Effects:**
- `MainMenu → Playing`: Emits InputUnlock, ScoreReset, LevelReset
- `Playing → Win/Lose`: Emits InputLock
- `Win/Lose → Replay`: Emits ScoreReset, LevelReset, then auto-transitions to Playing

### PelletManagerBridge.cs
Bridge component that connects `PelletManager` events to the `EventBus` system.

**Purpose:**
- Decouples PelletManager from GameStateManager
- Translates domain events to EventBus events

**Usage:**
Attach this component to the same GameObject as `PelletManager`. It will automatically:
- Subscribe to `PelletManager.OnAllPelletsCollectedEvent`
- Emit `EventBus.LevelCompleted` when all pellets are collected

## Testing

All systems follow Test-Driven Development (TDD) principles:
- Tests written first in `Assets/Tests/EditMode/`
- Implementation follows to pass tests
- 100% test coverage for core functionality

**Test Suites:**
- `EventBus_Tests.cs` - Tests EventBus functionality
- `GameState_Init_Tests.cs` - Tests initialization
- `GameState_Transition_Tests.cs` - Tests state transitions
- `GameState_Guards_Tests.cs` - Tests transition validation
- `GameState_Integration_Tests.cs` (PlayMode) - Integration tests
- `PelletManager_Integration_Tests.cs` (PlayMode) - PelletManager integration

## Architecture

This follows an **Event-Driven Architecture** pattern:
- Systems communicate through events, not direct references
- Loose coupling enables independent testing and development
- Clear separation of concerns
- Easy to extend with new systems

## Integration with Existing Systems

### GameManager
`GameManager` subscribes to:
- `EventBus.ScoreReset` - Resets score to 0
- `EventBus.LevelReset` - Resets level to 1 and lives to 3

### PelletManager
Connected via `PelletManagerBridge`:
- `PelletManager.OnAllPelletsCollectedEvent` → `EventBus.LevelCompleted`

### Future Integration Points
- **PlayerController**: Subscribe to `InputLock`/`InputUnlock`
- **UI System**: Subscribe to state changes for screen transitions
- **Ghost AI**: Subscribe to state changes for behavior control
- **Audio System**: Subscribe to events for sound effects
