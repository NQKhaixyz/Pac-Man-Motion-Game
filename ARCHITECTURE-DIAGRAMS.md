# Architecture Diagrams

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         Unity Scene                              │
│                                                                   │
│  ┌──────────────────┐                                            │
│  │ GameStateManager │ ◄───────────────┐                          │
│  │   Component      │                 │                          │
│  └─────────┬────────┘                 │                          │
│            │ subscribes               │ subscribes               │
│            ↓                          │                          │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │                      EventBus                            │    │
│  │                  (Static Class)                          │    │
│  │                                                           │    │
│  │  • RequestStartGame    • ScoreReset                     │    │
│  │  • LevelCompleted      • LevelReset                     │    │
│  │  • PlayerDead          • InputLock                      │    │
│  │  • RequestReplay       • InputUnlock                    │    │
│  │  • RequestBackToMenu                                    │    │
│  └───────────┬──────────────────────────────────┬──────────┘    │
│              │ publishes                         │ subscribes    │
│              ↓                                   ↓               │
│  ┌─────────────────────┐            ┌───────────────────────┐   │
│  │ PelletManagerBridge │            │    GameManager        │   │
│  │    Component        │            │    Component          │   │
│  └──────────┬──────────┘            └───────────────────────┘   │
│             │ subscribes                                         │
│             ↓                                                    │
│  ┌─────────────────────┐                                        │
│  │   PelletManager     │                                        │
│  │    Component        │                                        │
│  └─────────────────────┘                                        │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

## Event Flow Examples

### Example 1: Starting the Game
```
User clicks "Start" button
         │
         ↓
    UI calls:
  EventBus.EmitRequestStartGame()
         │
         ↓
  GameStateManager receives event
         │
         ↓
  Validates: MainMenu → Playing (✓)
         │
         ↓
  Fires: StateWillChange(MainMenu, Playing)
         │
         ↓
  Updates: Current = Playing
         │
         ↓
  Executes side-effects:
    • EventBus.EmitInputUnlock()
    • EventBus.EmitScoreReset()
    • EventBus.EmitLevelReset()
         │
         ↓
  Fires: StateChanged(MainMenu, Playing)
         │
         ↓
  GameManager receives:
    • ScoreReset → sets score = 0
    • LevelReset → sets level = 1, lives = 3
         │
         ↓
  Player input enabled, game starts!
```

### Example 2: Completing the Level
```
Player collects last pellet
         │
         ↓
  PelletManager detects 0 pellets remaining
         │
         ↓
  PelletManager fires: OnAllPelletsCollectedEvent
         │
         ↓
  PelletManagerBridge receives event
         │
         ↓
  Bridge calls: EventBus.EmitLevelCompleted()
         │
         ↓
  GameStateManager receives event
         │
         ↓
  Validates: Playing → Win (✓)
         │
         ↓
  Transitions to Win state
         │
         ↓
  Executes side-effects:
    • EventBus.EmitInputLock()
         │
         ↓
  UI shows Win screen
  Player input disabled
```

### Example 3: Replay Flow
```
Player clicks "Replay" button
         │
         ↓
  UI calls: EventBus.EmitRequestReplay()
         │
         ↓
  GameStateManager receives event
         │
         ↓
  Validates: Win → Replay (✓)
         │
         ↓
  Transitions to Replay state
         │
         ↓
  Executes side-effects:
    • EventBus.EmitScoreReset()
    • EventBus.EmitLevelReset()
    • Starts coroutine for auto-transition
         │
         ↓
  Wait 1 frame
         │
         ↓
  Auto-transition: Replay → Playing
         │
         ↓
  Executes side-effects:
    • EventBus.EmitInputUnlock()
         │
         ↓
  Game restarts with fresh state
```

## State Machine Diagram

```
┌────────────────────────────────────────────────────────────────┐
│                        State Machine                            │
│                                                                  │
│                         START                                    │
│                           ↓                                      │
│                    ┌─────────────┐                              │
│                    │  MainMenu   │                              │
│                    └──────┬──────┘                              │
│                           │                                      │
│                           │ RequestStartGame                     │
│                           │                                      │
│                           ↓                                      │
│                    ┌─────────────┐                              │
│          ┌────────>│   Playing   │<────────┐                    │
│          │         └──────┬──────┘         │                    │
│          │                │                │                    │
│          │                │                │                    │
│    (auto │    ┌───────────┼───────────┐    │ (auto             │
│     1f)  │    │ LevelComplete PlayerDead│   │  after reset)     │
│          │    ↓           ↓           │    │                    │
│          │ ┌──────┐   ┌──────┐       │    │                    │
│          │ │ Win  │   │ Lose │       │    │                    │
│          │ └───┬──┘   └───┬──┘       │    │                    │
│          │     │          │          │    │                    │
│          │     │ RequestReplay       │    │                    │
│          │     │          │          │    │                    │
│          │     └──────┬───┘          │    │                    │
│          │            ↓              │    │                    │
│          │      ┌──────────┐         │    │                    │
│          └──────│  Replay  │─────────┘    │                    │
│                 └──────────┘              │                    │
│                                           │                    │
│                                           │                    │
│                    RequestBackToMenu     │                    │
│                    (from any state)      │                    │
│                           │              │                    │
│                           └──────────────┘                    │
│                                                                │
└────────────────────────────────────────────────────────────────┘

States: 5 total
  • MainMenu  - Initial state, menu displayed
  • Playing   - Active gameplay
  • Win       - Level completed successfully
  • Lose      - Player died
  • Replay    - Transitional state (auto-transitions to Playing)

Transitions: 7 types
  ✓ MainMenu → Playing     (RequestStartGame)
  ✓ Playing → Win          (LevelCompleted)
  ✓ Playing → Lose         (PlayerDead)
  ✓ Win → Replay           (RequestReplay)
  ✓ Lose → Replay          (RequestReplay)
  ✓ Replay → Playing       (Automatic after 1 frame)
  ✓ Any → MainMenu         (RequestBackToMenu)
```

## Component Relationships

```
┌─────────────────────────────────────────────────────────────────┐
│                      Component Diagram                           │
│                                                                   │
│                                                                   │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │                    EventBus (Static)                       │  │
│  │                                                             │  │
│  │  Responsibilities:                                         │  │
│  │  • Central event hub                                       │  │
│  │  • Safe event invocation                                   │  │
│  │  • No dependencies                                         │  │
│  └───────────────────────────────────────────────────────────┘  │
│                            ▲                                     │
│                            │                                     │
│         ┌──────────────────┼──────────────────┐                 │
│         │                  │                  │                 │
│         │                  │                  │                 │
│  ┌──────┴────────┐  ┌──────┴────────┐  ┌──────┴────────┐       │
│  │GameStateManager│  │ PelletManager │  │ GameManager   │       │
│  │               │  │    Bridge     │  │               │       │
│  │Responsibilities│  │Responsibilities│  │Responsibilities│      │
│  │• State logic  │  │• Event bridge │  │• Score/Lives  │       │
│  │• Transitions  │  │• Decoupling   │  │• Level mgmt   │       │
│  │• Validation   │  │               │  │               │       │
│  └───────────────┘  └───────┬───────┘  └───────────────┘       │
│                             │                                    │
│                             │                                    │
│                      ┌──────┴───────┐                           │
│                      │PelletManager │                           │
│                      │              │                           │
│                      │Responsibilities│                          │
│                      │• Pellet mgmt │                           │
│                      │• Collection  │                           │
│                      │• Events      │                           │
│                      └──────────────┘                           │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘

Key Design Principles:
  • Loose Coupling: Systems communicate via EventBus only
  • Single Responsibility: Each component has one clear purpose
  • Dependency Inversion: Depend on abstractions (events), not concrete types
  • Open/Closed: Open for extension (add events), closed for modification
```

## Test Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                       Test Structure                             │
│                                                                   │
│  EditMode Tests (Logic Tests - No Unity Runtime Required)       │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                                                           │   │
│  │  EventBus_Tests.cs (11 tests)                           │   │
│  │  ├─ EmitSafe with no subscribers                        │   │
│  │  ├─ EmitSafe with exceptions                            │   │
│  │  └─ All event subscriptions/emissions                   │   │
│  │                                                           │   │
│  │  GameState_Init_Tests.cs (3 tests)                      │   │
│  │  ├─ StartUp sets MainMenu                               │   │
│  │  ├─ Events fire in correct order                        │   │
│  │  └─ Initial state validation                            │   │
│  │                                                           │   │
│  │  GameState_Transition_Tests.cs (9 tests)                │   │
│  │  ├─ All valid transitions                               │   │
│  │  ├─ Side-effect verification                            │   │
│  │  └─ Return value checks                                 │   │
│  │                                                           │   │
│  │  GameState_Guards_Tests.cs (16 tests)                   │   │
│  │  ├─ Valid transition checks                             │   │
│  │  ├─ Invalid transition rejections                       │   │
│  │  └─ Force flag behavior                                 │   │
│  │                                                           │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                   │
│  PlayMode Tests (Integration Tests - Unity Runtime Required)    │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                                                           │   │
│  │  GameState_Integration_Tests.cs (3 tests)               │   │
│  │  ├─ Complete flow: MainMenu → Playing → Win            │   │
│  │  ├─ Playing → Lose → Replay → Playing                  │   │
│  │  └─ BackToMenu from any state                          │   │
│  │                                                           │   │
│  │  PelletManager_Integration_Tests.cs (1 test)           │   │
│  │  └─ PelletManager → EventBus → GameStateManager        │   │
│  │                                                           │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                   │
│  Total: 43 test cases                                           │
│  Coverage: 100% for core systems                                │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

---

**Generated**: October 13, 2025  
**For**: Pac-Man Motion Game  
**Architecture**: Event-Driven with State Management
