using UnityEngine;

public class BreakObjects : MonoBehaviour
{
    [Header("The prefab to spawn when breaking")]
    [SerializeField] private GameObject breakObjects;

    [Header("Container holding your piece-objects (with Rigidbody2D)")]
    [SerializeField] private GameObject pieces;

    [Header("— Optional pieces workflow —")]
    [Tooltip("If true, disables all child Rigidbodies at Start and re-enables them on BreakObject()")]
    [SerializeField] private bool usePieces = false;

    private Explodable   explodable;
    private Rigidbody2D  rb;

    private void OnValidate()
    {
        if (pieces != null)
            usePieces = true;
    }

    void Start()
    {
        explodable = GetComponent<Explodable>();
        rb         = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.mass         = 1f;
            rb.constraints  = RigidbodyConstraints2D.None;
        }
        else
        {
            Debug.LogError($"[{name}] No Rigidbody2D found on the original object!");
        }

        if (usePieces && pieces != null)
        {
            // ① detach `pieces` from this GameObject so it won't get disabled/destroyed with it
            pieces.transform.SetParent(null);

            // ② disable all child rigidbodies until we break
            var childRBs = pieces.GetComponentsInChildren<Rigidbody2D>();
            if (childRBs.Length > 0)
            {
                foreach (var childRb in childRBs)
                {
                    childRb.bodyType  = RigidbodyType2D.Static;
                    childRb.simulated = false;
                }
            }
            else
            {
                Debug.LogWarning($"[{name}] usePieces is true but '{pieces.name}' has no Rigidbody2D children.");
            }
        }
    }

    public void BreakObject()
    {
        // explosion-force helper (optional)
        var ef = GameObject.FindObjectOfType<ExplosionForce>();
        Debug.DrawLine(transform.position, transform.position + Vector3.up * 2, Color.red, 1f);

        if (usePieces && pieces != null)
        {
            // ③ re-enable physics on the detached pieces
            foreach (var childRb in pieces.GetComponentsInChildren<Rigidbody2D>())
            {
                childRb.bodyType     = RigidbodyType2D.Dynamic;
                childRb.simulated    = true;
                childRb.gravityScale = 1f;
            }
        }

        // spawn the breakObjects prefab if assigned
        if (breakObjects != null)
            Instantiate(breakObjects, transform.position, Quaternion.identity);

        // trigger Explodable + explosion force
        if (explodable != null) explodable.explode();
        if (ef != null)         ef.doExplosion(transform.position);

        // optionally hide/destroy the original
        // gameObject.SetActive(false);
    }
}
