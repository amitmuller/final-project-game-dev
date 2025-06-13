using UnityEngine;

public class BreakObjects : MonoBehaviour
{
    [SerializeField] private GameObject breakObjects;
    private Explodable explodable;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        explodable = GetComponent<Explodable>();
        rb = GetComponent<Rigidbody2D>();
        
        // Ensure Rigidbody2D is set up correctly
        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.mass = 1f;
            rb.constraints = RigidbodyConstraints2D.None;
        }
        else
        {
            Debug.LogError($"No Rigidbody2D found on {gameObject.name}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BreakObject()
    {
        Debug.Log(gameObject);
        // Apply explosion force first
        ExplosionForce ef = GameObject.FindFirstObjectByType<ExplosionForce>();
        Debug.DrawLine(transform.position, transform.position + Vector3.up * 2, Color.red, 1f);

        ef.doExplosion(gameObject.transform.position);
        
        // Then break the object
        Instantiate(breakObjects, transform.position, Quaternion.identity);
        explodable.explode();
        
        // Optionally disable the original object
        // gameObject.SetActive(false);
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if (!collision.gameObject.CompareTag("Player")) return;
        // Instantiate(breakObjects, transform.position, Quaternion.identity);
        // gameObject.SetActive(false);
    }
}