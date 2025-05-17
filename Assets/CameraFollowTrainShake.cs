// CameraFollowTrainShake.cs
// Put this on the Main Camera (or the GameObject that holds it).

using UnityEngine;

[DisallowMultipleComponent]
public class CameraFollowTrainShake : MonoBehaviour
{
    //─────────────────────────  Target  ─────────────────────────
    [Header("Target")]
    [SerializeField] private Transform player;

    //─────────────────────────  Follow  ─────────────────────────
    [Header("Follow")]
    [Tooltip("World-space offset from the player (-10 z keeps an orthographic camera visible).")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Tooltip("0 = snap instantly, 1 = very slow. 0.1-0.25 feels good.")]
    [Range(0f, 1f)]
    [SerializeField] private float smoothness = 0.15f;

    //─────────────────────────  Bounds  ─────────────────────────
    [Header("Horizontal bounds (optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float leftLimit  = -25f;
    [SerializeField] private float rightLimit =  25f;

    //─────────────────────────  Shake  ──────────────────────────
    [Header("Train shake")]
    [SerializeField] private float shakeAmplitude = 0.05f; // world units
    [SerializeField] private float shakeFrequency = 5f;     // bumps/second
    [SerializeField] private bool  shakeZAxis    = false;   // depth wobble (for perspective cams)

    //─────────────────────────  Internals  ──────────────────────
    private float  noiseTime;

    //─────────────────────────────────────────────────────────────
    private void LateUpdate()
    {
        if (player == null) return;

        //---------------------------------------------------------
        // 1. Follow
        //---------------------------------------------------------
        Vector3 target = player.position + offset;

        if (useBounds)
            target.x = Mathf.Clamp(target.x, leftLimit, rightLimit);

        // Exponential smoothing: keeps motion frame-rate independent
        float t = 1f - Mathf.Pow(1f - smoothness, Time.deltaTime * 60f);
        transform.position = Vector3.Lerp(transform.position, target, t);

        //---------------------------------------------------------
        // 2. Shake (added on top of the follow position)
        //---------------------------------------------------------
        noiseTime += Time.deltaTime;
        float nx = Mathf.PerlinNoise(noiseTime * shakeFrequency, 0f) - 0.5f;
        float ny = Mathf.PerlinNoise(0f, noiseTime * shakeFrequency) - 0.5f;

        Vector3 shakeOffset = new Vector3(nx, ny, shakeZAxis ? nx : 0f) * (shakeAmplitude * 2f);
        transform.position += shakeOffset;
    }

#if UNITY_EDITOR
    // Draw the horizontal limits in the Scene view for convenience
    private void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        Gizmos.color = Color.cyan;
        float camHeight = Camera.main.orthographicSize * 2f;
        Gizmos.DrawLine(new Vector3(leftLimit,  camHeight, 0f), new Vector3(leftLimit, -camHeight, 0f));
        Gizmos.DrawLine(new Vector3(rightLimit, camHeight, 0f), new Vector3(rightLimit, -camHeight, 0f));
    }
#endif
}
