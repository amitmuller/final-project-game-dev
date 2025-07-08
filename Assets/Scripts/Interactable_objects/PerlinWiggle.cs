using UnityEngine;

namespace Interactable_objects
{
    [RequireComponent(typeof(Transform))]
    public class PerlinWiggle : MonoBehaviour
    {
        [Header("Wiggle Settings")] [Tooltip("How far from the start position it'll drift")] [SerializeField]
        private Vector2 amplitude = new Vector2(0.5f, 0.5f);

        [Tooltip("How fast the noise moves through time")] [SerializeField]
        private Vector2 speed = new Vector2(1f, 1.2f);

        [Tooltip("Different random seeds for X and Y axes")] [SerializeField]
        private Vector2 seed = new Vector2(0f, 100f);

        private Vector3 _startPos;

        private void Awake()
        {
            _startPos = transform.localPosition;
            // you can randomize seeds if you have many objects:
            seed.x = Random.Range(0f, 1000f);
            seed.y = Random.Range(0f, 1000f);
        }

        private void Update()
        {
            float t = Time.time;
            // sample Perlin noise at (seed + time*speed)
            float xNoise = Mathf.PerlinNoise(seed.x, t * speed.x) - 0.5f;
            float yNoise = Mathf.PerlinNoise(seed.y, t * speed.y) - 0.5f;

            // center around zero by subtracting 0.5, then scale by amplitude
            Vector3 offset = new Vector3(xNoise * amplitude.x,
                yNoise * amplitude.y,
                0f);

            transform.localPosition = _startPos + offset;
        }
    }
}