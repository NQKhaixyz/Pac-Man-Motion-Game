using UnityEngine;

/// <summary>
/// Quản lý input đơn giản cho cả project.
/// Đọc 2 trục chuẩn của Unity ("Horizontal", "Vertical") và chuyển thành
/// hướng di chuyển rời rạc 4 chiều (trái/phải/lên/xuống) dạng Vector2Int.
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    // Giá trị thô của 2 trục input. Khi dùng GetAxisRaw sẽ về -1, 0 hoặc 1.
    public Vector2 RawInput { get; private set; }

    // Hướng di chuyển rời rạc 4 chiều: x cho ngang, y cho dọc (trục z không dùng).
    public Vector2Int MoveDir { get; private set; }
    // Hướng vừa được nhấn (GetKeyDown) — useful to capture quick presses even khi axis đang giữ
    public Vector2Int PressedDir { get; private set; }

    void Awake()
    {
        // (VI) Awake()
        // - Mục đích: Thiết lập singleton cho InputManager, đảm bảo chỉ có 1 instance tồn tại.
        // - Hành vi: Nếu đã có instance khác thì huỷ GameObject này (tránh duplicate). Nếu chưa có,
        //   gán `Instance = this` và gọi `DontDestroyOnLoad` để giữ object khi load scene mới.
        // - Side-effects: Có thể huỷ object (Destroy/DestroyImmediate) nếu gặp duplicate; có log để debug.
        // - Lưu ý: Awake được gọi khi object được tạo (Editor hoặc Play). Đoạn cleanup cũng xoá
        //   các instance inactive/prefab trùng tên để tránh cảnh báo "Multiple managers".
        if (Instance != null && Instance != this)
        {
            // Destroy(Object obj):
            //  - obj: đối tượng cần huỷ; ở đây là gameObject gắn script InputManager bị trùng
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(Object target):
        //  - target: đối tượng cần giữ lại khi load scene mới; ở đây là chính InputManager
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Đọc trục ngang "Horizontal" và trục dọc "Vertical"
        // Input.GetAxisRaw(axisName):
        //  - axisName: tên trục được định nghĩa trong Project Settings > Input Manager
        float x = Input.GetAxisRaw("Horizontal");   // axisName = "Horizontal"
        float y = Input.GetAxisRaw("Vertical");     // axisName = "Vertical"
        RawInput = new Vector2(x, y);

        // Nếu nhấn cả 2 trục cùng lúc, ưu tiên ngang để giữ di chuyển 4 hướng mạch lạc
        if (Mathf.Abs(x) > 0.5f)
        {
            // Vector2Int(x, y):
            //  - x: -1 (trái) hoặc 1 (phải)
            //  - y: 0 vì đang di chuyển theo trục ngang
            MoveDir = new Vector2Int((int)Mathf.Sign(x), 0);
        }
        else if (Mathf.Abs(y) > 0.5f)
        {
            // Vector2Int(x, y):
            //  - x: 0 vì đang di chuyển theo trục dọc
            //  - y: -1 (xuống) hoặc 1 (lên)
            MoveDir = new Vector2Int(0, (int)Mathf.Sign(y));
        }
        else
        {
            MoveDir = Vector2Int.zero;
        }

        // Detect immediate key presses (GetKeyDown) for arrow keys and WASD so we can record
        // the player's intent even if the axis priority would hide it.
        PressedDir = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) PressedDir = new Vector2Int(1, 0);
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) PressedDir = new Vector2Int(-1, 0);
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) PressedDir = new Vector2Int(0, 1);
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) PressedDir = new Vector2Int(0, -1);
    }

    /// <summary>
    /// Có đang yêu cầu di chuyển (MoveDir khác (0,0)) hay không.
    /// </summary>
    public bool HasMovement() => MoveDir != Vector2Int.zero;
}
