// TrainCameraShake.cs
// Attach to your Camera (or the GameObject that already has your follow script).
// Works in both 2D and 3D projects.

using UnityEngine;

[DisallowMultipleComponent]
public class TrainCameraShake : MonoBehaviour
{
    [Header("Shake profile")]
    [Tooltip("How far (in world-units) the camera jitters. 0.02–0.08 feels right.")]
    [Range(0f, 0.2f)] [SerializeField] private float amplitude = 0.05f;
    [Tooltip("Bumps per second 1–10 is a good range.")]
    [Range(1f, 10f)]   [SerializeField] private float frequency = 5f;

    // Private state
    private Vector3 _startLocalPos;
    private float _time;


    private void Awake()
    {
        // Remember where the camera started **relative to its parent**.
        _startLocalPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        _time += Time.deltaTime;

        // Perlin noise gives smooth random numbers in [-0.5, +0.5].
        float noiseX = Mathf.PerlinNoise(_time * frequency, 0f) - 0.5f;
        float noiseY = Mathf.PerlinNoise(0f, _time * frequency) - 0.5f;

        var offset = new Vector3(noiseX, noiseY, 0f) * (amplitude * 2f);

        // Apply offset but keep original local space so it plays nicely with a follow script.
        transform.localPosition = _startLocalPos + offset;
    }

    private void OnDisable()
    {
        // Ensure the camera snaps back exactly when the script is turned off.
        transform.localPosition = _startLocalPos;
    }
}