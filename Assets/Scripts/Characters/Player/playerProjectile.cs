using UnityEngine;

namespace Characters.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerProjectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float speed = 15f;
        [SerializeField] private float lifetime = 7f;
        // [SerializeField] private GameObject hitEffect;

        private Rigidbody2D rb;
        private Vector2 moveDirection;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
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
                transform.localScale = new Vector3(-1f, 1f, 1f); // flip sprite if going left
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Optional: Add tag/layer checks here
            Debug.Log($"Projectile hit: {collision.name}");
            if (collision.gameObject.CompareTag("lightBolb"))
            {
                Destroy(collision.gameObject);
            }

            // if (hitEffect != null)
            // {
            //     Instantiate(hitEffect, transform.position, Quaternion.identity);
            // }

            Destroy(gameObject);
        }
    }
}