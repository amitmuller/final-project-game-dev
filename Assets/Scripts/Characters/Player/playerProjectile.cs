using UnityEngine;

namespace Characters.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerProjectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float speed = 15f;
        [SerializeField] private float lifetime = 7f;

        private Rigidbody2D rb;
        private Vector2 moveDirection;
        private Vector3 initialScale;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            initialScale = transform.localScale; // store prefab scale
        }

        private void Start()
        {
            rb.linearVelocity = moveDirection.normalized * speed;
            Destroy(gameObject, lifetime);
        }

        /// <summary>
        /// Sets the direction the projectile will travel.
        /// Call this right after instantiating it.
        /// </summary>
        public void SetDirection(Vector2 direction)
        {
            moveDirection = direction;

            if (direction.x < 0f)
                transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
            else
                transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"Projectile hit: {collision.name}");

            if (collision.CompareTag("lightBolb"))
            {
                Destroy(collision.gameObject);
            }

            Destroy(gameObject);
        }
    }
}