using UnityEngine;

/// <summary>
/// (VI) Controller nhân vật đơn giản, di chuyển theo lưới thô (grid).
/// Đọc hướng rời rạc từ InputManager rồi di chuyển Transform tới ô mục tiêu.
/// Dùng MoveTowards để di chuyển mượt giữa các ô.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("(VI) Tốc độ di chuyển (đơn vị/giây) giữa các ô lưới")]
    public float moveSpeed = 4f;

    [Tooltip("(VI) Kích thước 1 ô lưới theo đơn vị thế giới (world units)")]
    public float gridSize = 1f;

    // Current move target position (world space)
    Vector3 targetPos;
    // (VI) Hướng đang được queue (nhấn sớm khi đang di chuyển giữa các ô)
    Vector2Int queuedDir = Vector2Int.zero;
    // (VI) Hướng di chuyển hiện tại (lưu khi targetPos được set) để tham chiếu nếu cần
    Vector2Int currentDir = Vector2Int.zero;
    // (VI) Hướng được nhấn gần nhất và thời điểm nhấn — dùng để ưu tiên nút bấm sau cùng
    Vector2Int lastInputDir = Vector2Int.zero;
    float lastInputTime = 0f;
    // Theo dõi MoveDir trước đó từ InputManager để nhận sự thay đổi từ controller/axis
    Vector2Int prevImMoveDir = Vector2Int.zero;

    [Header("(VI) Tuỳ chọn kiểm tra tường")]
    [Tooltip("LayerMask tường để kiểm tra va chạm khi chọn ô kế tiếp (để tránh rẽ vào tường). Có thể để None nếu chưa dùng.")]
    public LayerMask wallLayer;
    [Tooltip("Bán kính khi kiểm tra va chạm điểm ô kế tiếp (OverlapCircle)")]
    public float probeRadius = 0.3f;

    void Start()
    {
        // (VI) Start()
        // - Mục đích: Khởi tạo trạng thái ban đầu của PlayerController.
        // - Hành vi chính:
        //   1) Snap vị trí hiện tại của transform về lưới (làm tròn theo gridSize) để tránh lệch ô.
        //   2) Gán targetPos = vị trí đã snap.
        //   3) Kiểm tra xem `InputManager.Instance` có tồn tại; nếu không có, log một cảnh báo.
        // - Side-effects: Thay đổi trực tiếp transform.position; nếu bạn dùng physics-based movement
        //   (Rigidbody2D) cần điều chỉnh để tránh xung đột.
        // - Lưu ý: Không tạo InputManager tại đây (để tránh spawn duplicates). Hãy đặt một InputManager
        //   cố định trong scene hoặc dùng một bootstrapper.
        // (VI) Snap vị trí bắt đầu về đúng lưới (làm tròn theo gridSize)
        Vector3 p = transform.position;
        p.x = Mathf.Round(p.x / gridSize) * gridSize;
        p.y = Mathf.Round(p.y / gridSize) * gridSize;
        p.z = 0f;
        transform.position = p;
        targetPos = p;

        // (VI) Kiểm tra InputManager có tồn tại không.
        // Không tự tạo InputManager ở đây để tránh spawn nhiều bản (gây duplicate).
        if (InputManager.Instance == null)
        {
            Debug.LogWarning("[PlayerController] No InputManager found in scene. Please add one (GameObject with InputManager) or have a single bootstrap create it.");
        }
    }

    void OnEnable()
    {
        // Đăng ký nghe sự kiện Replay chung của game (nếu có)
        // Khi Replay xảy ra, ta muốn reset player về lưới và trạng thái di chuyển.
        GameEvents.OnReplay += Reset;
    }

    void OnDisable()
    {
        // Huỷ đăng ký để tránh giữ reference khi object bị destroy
        GameEvents.OnReplay -= Reset;
    }

    /// <summary>
    /// (VI) Reset trạng thái Player: snap về lưới, dừng di chuyển và xóa mọi intent/queue.
    /// - Đây là hàm được gọi khi game phát sự kiện Replay.
    /// - Gọi public Reset() để tương thích với tên gọi ngắn gọn, nó chỉ gọi ResetToGrid().
    /// </summary>
    public void ResetToGrid()
    {
        // Snap vị trí hiện tại về lưới
        Vector3 p = transform.position;
        p.x = Mathf.Round(p.x / gridSize) * gridSize;
        p.y = Mathf.Round(p.y / gridSize) * gridSize;
        p.z = 0f;

        transform.position = p;
        targetPos = p;

        // Reset trạng thái input/movement
        queuedDir = Vector2Int.zero;
        currentDir = Vector2Int.zero;
        lastInputDir = Vector2Int.zero;
        prevImMoveDir = Vector2Int.zero;
        lastInputTime = 0f;
    }

    // Wrapper ngắn để dễ hook vào event gọi Reset
    public void Reset()
    {
        ResetToGrid();
    }

    void Update()
    {
        // (VI) Update()
        // - Mục đích: Xử lý input, quản lý queue cho hướng di chuyển và di chuyển mượt đến ô mục tiêu.
        // - Hành vi chính (theo thứ tự):
        //   1) Đọc input hiện tại từ `InputManager` và từ các phím (GetKeyDown/GetKey) để cập nhật
        //      `lastInputDir` (hướng bấm gần nhất) và `queuedDir` (hướng đang chờ áp dụng).
        //   2) Khi đã tới `targetPos` (tâm ô), cố gắng áp dụng `queuedDir` (thuật toán ưu tiên nút bấm sau cùng).
        //   3) Trước khi áp dụng turn, kiểm tra ô đích (nextPos) có bị chặn bởi `wallLayer` không; nếu bị
        //      chặn thì giữ `queuedDir` để thử lại ở ô tiếp theo.
        //   4) Luôn di chuyển mượt về `targetPos` bằng `Vector3.MoveTowards`.
        // - Side-effects: Thay đổi `targetPos`, `currentDir`, `queuedDir` và trực tiếp thay đổi `transform.position`.
        // - Lưu ý quan trọng: `Update()` polling cả GetKeyDown tại PlayerController để tránh phụ thuộc
        //   vào thứ tự Update giữa component; `queuedDir` được giữ cho đến khi có thể áp dụng.
        // - Edge cases: Nếu map không khớp lưới (gridSize) hoặc colliders không chính xác, turn có thể thất bại.
        //   Hãy đảm bảo map và player snap cùng `gridSize`.
        var im = InputManager.Instance;
        // Nếu có input mới thì queue nó (luôn nhận input dù đang di chuyển)
        // Poll keydown locally as well to avoid missing quick presses if InputManager.Update
        // runs after PlayerController.Update in the same frame.
        Vector2Int localPressed = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) localPressed = new Vector2Int(1, 0);
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) localPressed = new Vector2Int(-1, 0);
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) localPressed = new Vector2Int(0, 1);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) localPressed = new Vector2Int(0, -1);

        // Tính hướng đang được giữ (held) mỗi frame
        bool upHeld = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        bool downHeld = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        bool rightHeld = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        bool leftHeld = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);

        // Cập nhật lastInputDir khi có sự kiện nhấn mới
        if (localPressed != Vector2Int.zero)
        {
            lastInputDir = localPressed;
            lastInputTime = Time.time;
        }
        else if (im != null && im.PressedDir != Vector2Int.zero)
        {
            lastInputDir = im.PressedDir;
            lastInputTime = Time.time;
        }
        else if (im != null && im.MoveDir != prevImMoveDir && im.MoveDir != Vector2Int.zero)
        {
            // axis/controller changed direction
            lastInputDir = im.MoveDir;
            lastInputTime = Time.time;
        }

        // Update prevImMoveDir for next frame
        if (im != null) prevImMoveDir = im.MoveDir;

        // Nếu lastInputDir không còn được giữ (keyboard) hoặc không khớp với MoveDir, clear nó
        if (lastInputDir != Vector2Int.zero)
        {
            bool stillHeld = false;
            if (lastInputDir.x != 0)
            {
                stillHeld = (lastInputDir.x > 0) ? rightHeld : leftHeld;
            }
            else if (lastInputDir.y != 0)
            {
                stillHeld = (lastInputDir.y > 0) ? upHeld : downHeld;
            }
            // Nếu không còn held nhưng InputManager axis points to same dir, cũng coi là stillHeld
            if (!stillHeld && im != null && im.MoveDir == lastInputDir) stillHeld = true;

            if (!stillHeld)
            {
                // Key released and no axis holding it -> clear last input
                lastInputDir = Vector2Int.zero;
            }
        }

        // Desired direction is the most recent press (lastInputDir) when available; fallback to held directions or axis
        Vector2Int desired = Vector2Int.zero;
        if (lastInputDir != Vector2Int.zero)
        {
            desired = lastInputDir;
        }
        else if (upHeld) desired = new Vector2Int(0, 1);
        else if (downHeld) desired = new Vector2Int(0, -1);
        else if (rightHeld) desired = new Vector2Int(1, 0);
        else if (leftHeld) desired = new Vector2Int(-1, 0);
        else if (im != null && im.HasMovement()) desired = im.MoveDir;

        if (desired != Vector2Int.zero)
        {
            queuedDir = desired;
        }

        // Nếu đã tới ô mục tiêu, áp dụng hướng đã queue (ưu tiên) hoặc hướng mới
        if (Vector3.Distance(transform.position, targetPos) < 0.001f)
        {
            Vector2Int dirToApply = Vector2Int.zero;
            if (queuedDir != Vector2Int.zero)
            {
                dirToApply = queuedDir;
            }
            else if (im != null && im.HasMovement())
            {
                dirToApply = im.MoveDir;
            }

            if (dirToApply != Vector2Int.zero)
            {
                Vector3 nextPos = targetPos + new Vector3(dirToApply.x * gridSize, dirToApply.y * gridSize, 0f);

                // Kiểm tra có thể đi vào ô desired không. Nếu có wallLayer được gán và có va chạm,
                // không áp dụng turn ngay, giữ queuedDir để thử ở ô sau.
                bool blocked = false;
                if (wallLayer != 0)
                {
                    Collider2D hit = Physics2D.OverlapCircle(nextPos, probeRadius, wallLayer);
                    blocked = (hit != null);
                }

                if (!blocked)
                {
                    targetPos = nextPos;
                    currentDir = dirToApply;
                    // đã dùng queued dir, clear
                    queuedDir = Vector2Int.zero;
                }
                else
                {
                    // Bị chặn: giữ queuedDir để thử lại ở ô kế
                    // Không đổi currentDir/targetPos
                }
            }
            else
            {
                // không có hướng nào => đứng im
                currentDir = Vector2Int.zero;
            }
        }

        // (VI) Di chuyển dần tới targetPos bằng Vector3.MoveTowards(current, target, maxDistanceDelta)
        //  - current: transform.position hiện tại
        //  - target:  targetPos (ô mục tiêu)
        //  - maxDistanceDelta: moveSpeed * Time.deltaTime (khoảng cách tối đa có thể đi trong 1 frame)
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        // (VI) OnDrawGizmos()
        // - Mục đích: Vẽ debug gizmo cho ô mục tiêu (targetPos) để dễ quan sát trong Scene view.
        // - Hiển thị: một wire sphere màu vàng tại tâm ô mục tiêu hoặc vị trí hiện tại nếu target chưa set.
        Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(center, radius):
        //  - center: tâm hình cầu dây; nếu targetPos chưa thiết lập, dùng transform.position
        //  - radius: bán kính hiển thị
        Gizmos.DrawWireSphere(targetPos == default ? transform.position : targetPos, 0.15f);
    }
}
