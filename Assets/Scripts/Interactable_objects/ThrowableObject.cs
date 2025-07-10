using Light;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;
    public Color highlightColor = Color.yellow;
    private GameObject indicatorInstance;
    public bool IsHeld { get; set; } = false;
    private ParticleSystem noiseParticles;
    private Vector3 initialPosition;
    private LayerMask originalLayer;
    private Transform initialParent;
    private Explodable _explodable;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        _explodable = GetComponent<Explodable>();
        originalColor = sr.color;

        // Preload noisePS prefab and instantiate a single ParticleSystem instance to avoid runtime instantiation delay
        var noisePrefab = Resources.Load<ParticleSystem>("Sound ripple");
        if (noisePrefab != null)
        {
            noiseParticles = Instantiate(noisePrefab, transform.position, Quaternion.identity);
            noiseParticles.gameObject.name = "noisePS_Instance";

            // IMMEDIATELY OVERRIDE ANY PREFAB DELAY:
            var main = noiseParticles.main;
            main.startDelay = 0f;

            noiseParticles.Stop();
            noiseParticles.gameObject.SetActive(false);
            noiseParticles.Simulate(0.001f, withChildren: true, restart: true);
            noiseParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        else
        {
            Debug.LogError("[ThrowableObject] Couldn’t load 'noisePS' prefab from Resources", this);
        }

        // Load indicator prefab once
        var prefab = Resources.Load<GameObject>("takeThrowIcon");
        if (prefab)
        {
            indicatorInstance = Instantiate(prefab, transform);
            indicatorInstance.transform.localPosition = new Vector3(0, 1f, 0);
            indicatorInstance.SetActive(false);
        }
        else
        {
            Debug.LogError("[ThrowableObject] Couldn't load 'takeThrowIcon' prefab from Resources", this);
        }

        // Store initial state
        initialPosition = transform.position;
        originalLayer = gameObject.layer;
        initialParent = transform.parent;
    }

    public void Highlight(bool enable)
    {
        sr.color = enable ? highlightColor : originalColor;
        if (indicatorInstance != null)
            indicatorInstance.SetActive(enable);
    }
    
    public void GrabObject()
    {
        if (indicatorInstance != null)
            indicatorInstance.SetActive(false);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        print("collided with1"+ collision.gameObject.name);
        
        if (collision.gameObject.CompareTag("lightBolb"))
        {
            var lamp = collision.gameObject.GetComponent<LighBulb>();
            if (lamp != null)
                lamp.AlertNearbyEnemies();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("ground"))
        {
            print("collided with floor");
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }

        NoiseManager.RaiseNoise(transform.position);

        // Play preloaded noise particles without delay
        if (noiseParticles != null)
        {
            noiseParticles.transform.position = transform.position;
            noiseParticles.gameObject.SetActive(true);

            // ensure no old delay or particles remain
            noiseParticles.Clear(true);
            noiseParticles.Play();
        }

        if (_explodable != null)
        {
            BreakObject();
            _explodable.explode();
        }
        
        gameObject.layer = LayerMask.NameToLayer("notCollide");
        sr.sortingOrder = 11;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("lightBolb"))
        {
            var lamp = other.gameObject.GetComponent<LighBulb>();
            if (lamp != null)
                lamp.AlertNearbyEnemies();

            NoiseManager.RaiseNoise(other.transform.position);
            Destroy(other.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 5);
    }

    public void reset()
    {
        transform.SetParent(initialParent);
        transform.position = initialPosition;
        gameObject.layer = originalLayer;
        var rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.None;
        rb.isKinematic = true;

        // Reset noise particle state
        if (noiseParticles != null)
        {
            noiseParticles.Stop();
            noiseParticles.gameObject.SetActive(false);
        }
    }
    
    public void BreakObject()
    {
        var ef = GameObject.FindObjectOfType<ExplosionForce>();
        if (ef != null)
            ef.doExplosion(transform.position);
    }
}
