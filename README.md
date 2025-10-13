# Pac-Man-Motion-Game
Chơi Pac-Man bằng *chuyển động cơ thể*, không cần chạm màn hình.   
Ứng dụng sử dụng *camera + computer vision* để nhận diện hành động và điều khiển nhân vật trong game.

## 🎮 Giới thiệu

Pac-Man Motion Game là một phiên bản sáng tạo của trò chơi Pac-Man kinh điển, nơi người chơi điều khiển nhân vật bằng chuyển động cơ thể thay vì phím hoặc nút bấm. Game khuyến khích người chơi vận động nhiều hơn, kết hợp giải trí và thể dục.

## 🚀 Ý tưởng

- Người chơi đứng trước camera
- Camera thu hình → AI nhận diện dáng người (pose detection)
- Dữ liệu pose được chuyển thành lệnh (UP/DOWN/LEFT/RIGHT)
- Pac-Man trong game di chuyển theo đúng hành động cơ thể

👉 **Mục tiêu**: Khuyến khích người chơi *vận động nhiều hơn* thay vì ngồi yên một chỗ.

## 🛠 Công nghệ sử dụng

- **Unity**: Xây dựng game Pac-Man và UI
- **TensorFlow Lite / MediaPipe / MoveNet**: Nhận diện pose
- **C# / Unity ML**: Xử lý input từ camera
- **Android/iOS build**: Chạy trên điện thoại
- **GitHub Actions**: CI/CD build tự động

## 📁 Cấu trúc dự án

```
Pac-Man-Motion-Game/
├── Assets/
│   ├── Scenes/
│   │   └── GameScene.unity          # Scene chính của game
│   ├── Scripts/
│   │   ├── Core/                    # Core game systems
│   │   │   ├── EventBus.cs          # Event bus trung tâm
│   │   │   ├── GameStateManager.cs  # Quản lý game states
│   │   │   └── PelletManagerBridge.cs # Bridge PelletManager-EventBus
│   │   ├── GameManager.cs           # Quản lý trạng thái game
│   │   ├── MapGenerator.cs          # Tạo bản đồ Pac-Man
│   │   ├── Pellet.cs                # Logic chấm ăn
│   │   └── PelletManager.cs         # Quản lý các chấm ăn
│   ├── Tests/                       # Unit & Integration tests
│   │   ├── EditMode/                # Tests không cần Unity runtime
│   │   └── PlayMode/                # Tests cần Unity runtime
│   └── Prefabs/                     # Các prefab cho game objects
├── ProjectSettings/                  # Cài đặt Unity project
└── README.md
```

## 🎯 Tính năng đã triển khai (P1-01)

### ✅ Bản đồ game (MapGenerator.cs)
- Tạo bản đồ Pac-Man 28x31 ô
- Layout mê cung kinh điển với tường và lối đi
- Khu vực ghost house ở giữa
- Tự động căn chỉnh bản đồ vào trung tâm màn hình
- Hệ thống grid-based với các helper methods:
  - `GridToWorldPosition()`: Chuyển đổi tọa độ grid sang world
  - `IsWalkable()`: Kiểm tra ô có thể đi qua được không

### ✅ Hệ thống chấm ăn (Pellet.cs)
- **Chấm thường** (Normal Pellet): 10 điểm
- **Chấm năng lượng** (Power Pellet): 50 điểm, to và màu vàng
- Tự động phát hiện va chạm với Pac-Man
- Visual customization: màu sắc, kích thước
- Tự động tạo sprite hình tròn nếu không có sẵn

### ✅ Quản lý chấm ăn (PelletManager.cs)
- Spawn và quản lý tất cả chấm ăn trong game
- Theo dõi số lượng chấm đã ăn và còn lại
- Tính điểm tự động
- Event system:
  - `OnPelletCollectedEvent`: Khi ăn được chấm
  - `OnAllPelletsCollectedEvent`: Khi ăn hết chấm (hoàn thành level)
- Methods hữu ích:
  - `SpawnPellet()`: Tạo chấm mới
  - `GetRemainingPellets()`: Số chấm còn lại
  - `GetTotalScore()`: Tổng điểm

### ✅ GameManager.cs
- Quản lý trạng thái game (score, lives, level)
- Singleton pattern để truy cập global
- Xử lý logic level complete
- Tích hợp với MapGenerator và PelletManager
- Chuẩn bị sẵn cho việc mở rộng:
  - UI connection points
  - Pause/Resume functionality
  - Life system
  - Level progression

### ✅ Game State Manager & Event Bus (Core Systems)
- **EventBus**: Hệ thống event bus trung tâm cho giao tiếp giữa các systems
  - Events: RequestStartGame, LevelCompleted, PlayerDead, RequestReplay, RequestBackToMenu
  - Events nội bộ: ScoreReset, LevelReset, InputLock, InputUnlock
  - Safe event invocation với exception handling
  - Test-friendly với ClearAllEvents()
  
- **GameStateManager**: Quản lý trạng thái game với các transitions hợp lệ
  - States: MainMenu → Playing → Win/Lose → Replay → Playing
  - Validation cho state transitions
  - Auto-transition từ Replay sang Playing
  - Side-effects tự động (lock/unlock input, reset score/level)
  - Events: StateWillChange, StateChanged
  
- **PelletManagerBridge**: Kết nối PelletManager với EventBus
  - Chuyển đổi PelletManager events sang EventBus events
  - Decoupling giữa các systems

- **TDD Implementation**: 100% test coverage
  - EditMode tests cho logic core
  - PlayMode tests cho integration
  - 4 test suites với 30+ test cases

Xem thêm chi tiết tại [`Assets/Scripts/Core/README.md`](Assets/Scripts/Core/README.md)

## 🎮 Cách sử dụng

### Yêu cầu
- Unity 2021.3.0f1 hoặc cao hơn
- Hỗ trợ 2D rendering

### Mở project
1. Clone repository:
   ```bash
   git clone https://github.com/NQKhaixyz/Pac-Man-Motion-Game.git
   ```
2. Mở Unity Hub
3. Chọn "Open" và chọn thư mục dự án
4. Mở scene `Assets/Scenes/GameScene.unity`

### Chạy game
1. Trong Unity Editor, mở scene `GameScene.unity`
2. Nhấn Play button
3. Bản đồ và chấm ăn sẽ tự động được tạo

## 🔄 Các bước tiếp theo (Roadmap)

- [ ] **P1-02**: Tạo nhân vật Pac-Man và hệ thống di chuyển cơ bản
- [ ] **P1-03**: Triển khai camera integration và pose detection
- [ ] **P1-04**: Tích hợp AI để chuyển đổi pose thành lệnh di chuyển
- [ ] **P1-05**: Tạo ghost AI và logic chase/scatter
- [ ] **P1-06**: Thêm UI (score, lives, level)
- [ ] **P1-07**: Thêm âm thanh và hiệu ứng
- [ ] **P1-08**: Build cho Android/iOS
- [ ] **P1-09**: Tối ưu hóa performance

## 🤝 Đóng góp

Mọi đóng góp đều được chào đón! Hãy tạo issue hoặc pull request.

## 📄 License

MIT License - xem file [LICENSE](LICENSE) để biết thêm chi tiết.
