// PushableObject.cs
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    [Header("Fall Settings")]
    [Tooltip("Sorting layer to switch into when falling")]
    [SerializeField] private string playerSortingLayer = "Player";
    [Tooltip("Gravity scale to apply once falling")]
    [SerializeField] private float fallGravityScale = 2f;

    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private bool _isFalling;

    void Awake()
    {
        _rigidbody2D   = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _rigidbody2D.bodyType     = RigidbodyType2D.Kinematic;
        _rigidbody2D.gravityScale = 0f;
    }

    /// <summary>
    /// Called by TailPushController when tail hits this object.
    /// </summary>
    public void TriggerFall()
    {
        if (_isFalling) return;
        _isFalling = true;

        // Switch to Player layer so it's visible above walls, etc.
        _spriteRenderer.sortingLayerName = playerSortingLayer;

        // Let physics drop it
        _rigidbody2D.bodyType     = RigidbodyType2D.Dynamic;
        _rigidbody2D.gravityScale = fallGravityScale;

        // Notify enemies within range
        // NoiseManager.RaiseNoise(transform.position);
        Debug.Log($"[PushableObject] NoiseRaised at {transform.position}");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isFalling) return;

        // Destroy when hitting the ground
        if (collision.collider.CompareTag("ground"))
            Destroy(gameObject);
    }
}