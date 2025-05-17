using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform player;      
    [Header("Smoothing 0 = snap, 1 = slow")]
    [Range(0f, 1f)][SerializeField] float timeOffset = 0.15f;
    [Tooltip("Offset from player in X/Y (rig’s Z stays at –10)")]
    [SerializeField] Vector3 offsetPosition = new Vector3(0f, 0f, 0f);
    [Header("Horizontal Bounds")]
    [SerializeField] float minX = -10f;     // left boundary
    [SerializeField] float maxX =  10f;     // right boundary

    public bool following = true;

    void LateUpdate()
    {
        if (player == null || !following) return;

        // current camera pos
        Vector3 startPos = transform.position;

        // desired camera pos = player + offset, but keep Y/Z locked
        Vector3 targetPos = player.position + offsetPosition;
        targetPos.y = startPos.y;
        targetPos.z = startPos.z;

        // smooth lerp
        float t = 1f - Mathf.Pow(1f - timeOffset, Time.deltaTime * 30f);
        Vector3 smoothed = Vector3.Lerp(startPos, targetPos, t);

        // clamp horizontally
        smoothed.x = Mathf.Clamp(smoothed.x, minX, maxX);

        // assign
        transform.position = smoothed;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // choose a long enough Y‐range to cover your scene:
        float topY    = transform.position.y + 20f;
        float bottomY = transform.position.y - 20f;

        // left line
        Gizmos.DrawLine(
            new Vector3(minX, bottomY, 0),
            new Vector3(minX, topY,    0)
        );

        // right line
        Gizmos.DrawLine(
            new Vector3(maxX, bottomY, 0),
            new Vector3(maxX, topY,    0)
        );
    }
}