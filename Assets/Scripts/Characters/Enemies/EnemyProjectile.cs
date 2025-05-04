using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 direction;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;

        // Optional: rotate the projectile to face movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime); // move forward in local space
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            characterMovement pm = other.GetComponent<characterMovement>();
            if (pm != null)
            {
                pm.ApplySlow();
            }

            Destroy(gameObject);
        }
    }
}