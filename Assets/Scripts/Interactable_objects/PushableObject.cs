using UnityEngine;

public class PushableObject : MonoBehaviour
{
    [SerializeField] private string playerLayer = "Player";
    [SerializeField] private float fallGravity = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isFalling;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    public void TriggerFall()
    {
        Debug.Log("Falling");
        if (isFalling) return;
        isFalling = true;
        sr.sortingLayerName = playerLayer;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isFalling) return;
        Debug.Log("Collided with " + col.gameObject.name);
        if (col.collider.CompareTag("ground"))
            Destroy(gameObject);
    }
}