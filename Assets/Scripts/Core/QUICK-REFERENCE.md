# Quick Reference: GameState Manager & EventBus

## 🚀 Quick Start

### 1. Setup in Unity Scene
```
1. Create empty GameObject named "GameStateManager"
2. Add GameStateManager component
3. In script or Start(), call: gameStateManager.StartUp()
```

### 2. Basic Event Usage
```csharp
// In your MonoBehaviour:
void OnEnable()
{
    EventBus.LevelCompleted += OnLevelComplete;
}

void OnDisable()
{
    EventBus.LevelCompleted -= OnLevelComplete; // IMPORTANT!
}

void OnLevelComplete()
{
    Debug.Log("Level complete!");
}
```

---

## 📋 Event Reference

### Game Flow Events
| Event | When to Emit | Effect |
|-------|--------------|--------|
| `RequestStartGame` | Player clicks "Start" | MainMenu → Playing |
| `LevelCompleted` | All pellets collected | Playing → Win |
| `PlayerDead` | Player dies | Playing → Lose |
| `RequestReplay` | Player clicks "Replay" | Win/Lose → Replay → Playing |
| `RequestBackToMenu` | Player clicks "Menu" | Any → MainMenu |

### System Events
| Event | When Emitted | Use For |
|-------|--------------|---------|
| `ScoreReset` | Game starts/replays | Reset score to 0 |
| `LevelReset` | Game starts/replays | Reset level to 1 |
| `InputLock` | Level ends (Win/Lose) | Disable player input |
| `InputUnlock` | Game starts | Enable player input |

---

## 🎮 State Transitions

```
Valid Transitions:
MainMenu ──────> Playing     ✓
Playing  ──────> Win         ✓
Playing  ──────> Lose        ✓
Win      ──────> Replay      ✓
Lose     ──────> Replay      ✓
Replay   ──────> Playing     ✓ (automatic)
Any      ──────> MainMenu    ✓

Invalid Transitions:
Win      ──────> Lose        ✗
Playing  ──────> Replay      ✗
MainMenu ──────> Win         ✗
```

---

## 💡 Common Patterns

### Pattern 1: UI Button Click
```csharp
public void OnStartButtonClicked()
{
    EventBus.EmitRequestStartGame();
}
```

### Pattern 2: Check State Before Action
```csharp
if (gameStateManager.Current == GameState.Playing)
{
    // Allow player movement
}
```

### Pattern 3: React to State Changes
```csharp
void OnEnable()
{
    gameStateManager.StateChanged += OnStateChanged;
}

void OnStateChanged(GameState from, GameState to)
{
    if (to == GameState.Playing)
    {
        ShowGameUI();
    }
    else if (to == GameState.MainMenu)
    {
        ShowMainMenu();
    }
}
```

### Pattern 4: Multiple Events
```csharp
void OnEnable()
{
    EventBus.ScoreReset += ResetScore;
    EventBus.LevelReset += ResetLevel;
    EventBus.InputLock += DisableInput;
}
```

---

## ⚠️ Common Mistakes to Avoid

### ❌ Forgetting to Unsubscribe
```csharp
// BAD - Memory leak!
void OnEnable()
{
    EventBus.LevelCompleted += OnLevelComplete;
}
// Missing OnDisable!
```

### ✅ Correct Way
```csharp
void OnEnable()
{
    EventBus.LevelCompleted += OnLevelComplete;
}

void OnDisable()
{
    EventBus.LevelCompleted -= OnLevelComplete; // Always unsubscribe!
}
```

### ❌ Direct State Manipulation
```csharp
// BAD - Don't modify state directly!
gameStateManager.Current = GameState.Playing; // This won't work!
```

### ✅ Correct Way
```csharp
// Use TransitionTo() or emit event
gameStateManager.TransitionTo(GameState.Playing);
// or
EventBus.EmitRequestStartGame();
```

---

## 🔧 Testing

### Testing Individual Systems
```csharp
[Test]
public void MyTest()
{
    // Clear events before test
    EventBus.ClearAllEvents();
    
    // Your test code
    bool called = false;
    EventBus.LevelCompleted += () => called = true;
    EventBus.EmitLevelCompleted();
    
    Assert.IsTrue(called);
}
```

### Testing State Transitions
```csharp
[UnityTest]
public IEnumerator TestStateTransition()
{
    var gsm = gameObject.AddComponent<GameStateManager>();
    gsm.StartUp();
    
    EventBus.EmitRequestStartGame();
    yield return null; // Wait one frame
    
    Assert.AreEqual(GameState.Playing, gsm.Current);
}
```

---

## 📝 Cheat Sheet

### Subscribe to Events
```csharp
EventBus.EventName += HandlerMethod;
```

### Emit Events
```csharp
EventBus.EmitEventName();
```

### Check Current State
```csharp
gameStateManager.Current == GameState.Playing
```

### Transition State
```csharp
gameStateManager.TransitionTo(GameState.Win);
```

### Validate Transition
```csharp
if (gameStateManager.CanTransition(from, to))
{
    // Transition is valid
}
```

---

## 🎯 Integration Checklist

When integrating a new system:

- [ ] Subscribe to relevant events in `OnEnable()`
- [ ] Unsubscribe in `OnDisable()`
- [ ] Handle null cases for GameStateManager reference
- [ ] Test with different state transitions
- [ ] Add Context Menu methods for testing
- [ ] Document which events your system uses

---

## 🔗 See Also

- [Core Systems README](README.md) - Detailed documentation
- [ExampleStateUsage.cs](ExampleStateUsage.cs) - Complete example
- [TDD-GAMESTATE-SUMMARY.md](../../../TDD-GAMESTATE-SUMMARY.md) - Implementation summary

---

## 🆘 Troubleshooting

### Events not firing?
- Check if you subscribed in `OnEnable()`
- Make sure EventBus is not cleared
- Verify event is being emitted

### State not changing?
- Check if transition is valid with `CanTransition()`
- Look for error logs in Console
- Verify GameStateManager is initialized with `StartUp()`

### Memory leaks?
- Always unsubscribe in `OnDisable()`
- Use `FindAnyObjectByType<>()` instead of storing static references
- Don't subscribe in `Awake()` or `Start()` without unsubscribing

---

**Last Updated**: October 13, 2025  
**Version**: 1.0.0
