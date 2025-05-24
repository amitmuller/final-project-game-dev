using Light;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;
    public Color highlightColor = Color.yellow;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void Highlight(bool enable)
    {
        sr.color = enable ? highlightColor : originalColor;
    }

    public void fall()
    {
        NoiseManager.RaiseNoise(transform.position);
        Debug.Log($"[PushableObject] NoiseRaised at {transform.position}");
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // You can filter with collision.gameObject.tag if needed
        fall();
        if (collision.gameObject.CompareTag("lightBolb"))
        {
            var lamp = collision.gameObject.GetComponent<LighBulb>();
            if (lamp != null)
            {
                lamp.AlertNearbyEnemies();
            }

            Destroy(collision.gameObject);
        }

    }
}

