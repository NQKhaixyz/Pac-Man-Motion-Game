using UnityEngine;

/// <summary>
/// (VI) Overlay debug đơn giản để kiểm tra nhanh input và vị trí player khi chạy Play Mode.
/// Gắn script này vào một GameObject bất kỳ trong scene.
/// </summary>
public class InputDebugOverlay : MonoBehaviour
{
    [Tooltip("(VI) Transform của nhân vật để hiển thị toạ độ. Nếu để trống sẽ thử tìm theo tag 'Player'.")]
    public Transform target;

    void Awake()
    {
        if (target == null)
        {
            var t = GameObject.FindGameObjectWithTag("Player");
            if (t != null) target = t.transform;
        }
    }

    void OnGUI()
    {
        var im = InputManager.Instance;
        string moveDir = im != null ? $"({im.MoveDir.x}, {im.MoveDir.y})" : "(no IM)";
        string raw = im != null ? $"({im.RawInput.x:F1}, {im.RawInput.y:F1})" : "(no IM)";
        string pos = target != null ? target.position.ToString("F2") : "(no target)";

        // GUI.Label(positionRect, text):
        //  - positionRect: vị trí & kích thước label (pixel) trên màn hình
        //  - text: nội dung hiển thị
        GUI.Label(new Rect(10, 10, 420, 20), $"MoveDir: {moveDir}  Raw: {raw}");
        GUI.Label(new Rect(10, 30, 420, 20), $"Player Pos: {pos}");
    }
}
