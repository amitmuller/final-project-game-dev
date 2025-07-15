using Light;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum MaterialType
{
    Glass,
    Wood,
    Stone
}
public class ThrowableObject : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;
    public Color highlightColor = Color.yellow;
    public bool IsHeld { get; set; } = false;
    // private ParticleSystem noiseParticles;
    private Vector3 initialPosition;
    private LayerMask originalLayer;
    private Transform initialParent;
    private Explodable _explodable;
    [SerializeField] private GameObject grabUI;
    [SerializeField] private GameObject grabUICircle;
    [Header("What is this made of?")]
    public MaterialType materialType = MaterialType.Glass;
    private Light2D _light;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        _explodable = GetComponent<Explodable>();
        originalColor = sr.color;
        
        Transform oldParent = transform;
        grabUI.transform.parent = null;
        grabUI.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        grabUI.transform.parent = oldParent;
        
        
        grabUICircle.transform.localPosition = Vector3.zero;
        grabUICircle.transform.localScale = transform.localScale;
        grabUICircle.GetComponent<SpriteRenderer>().sortingOrder = sr.sortingOrder-1;
        grabUI.SetActive(false);
        grabUICircle.SetActive(false);
        _light = GetComponent<Light2D>();
        _light.enabled = false;
        
        // Preload noisePS prefab and instantiate a single ParticleSystem instance to avoid runtime instantiation delay
        var noisePrefab = Resources.Load<ParticleSystem>("Sound ripple");
        // if (noisePrefab != null)
        // {
        //     noiseParticles = Instantiate(noisePrefab, transform.position, Quaternion.identity);
        //     noiseParticles.gameObject.name = "noisePS_Instance";
        //
        //     // IMMEDIATELY OVERRIDE ANY PREFAB DELAY:
        //     var main = noiseParticles.main;
        //     main.startDelay = 0f;
        //
        //     noiseParticles.Stop();
        //     noiseParticles.gameObject.SetActive(false);
        //     noiseParticles.Simulate(0.001f, withChildren: true, restart: true);
        //     noiseParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        // }
        // else
        // {
        //     Debug.LogError("[ThrowableObject] Couldn’t load 'noisePS' prefab from Resources", this);
        // }

        // Store initial state
        initialPosition = transform.position;
        originalLayer = gameObject.layer;
        initialParent = transform.parent;
    }
    

    public void Highlight(bool enable)
    {
        // sr.color = enable ? highlightColor : originalColor;
        // _light.enabled = enable;
        if (grabUI != null)
            grabUI.SetActive(enable);
        if (grabUICircle != null)
            grabUICircle.SetActive(enable);
    }
    
    public void GrabObject()
    {
        if (grabUI != null)
            grabUI.SetActive(false);
        if (grabUICircle != null)
            grabUICircle.SetActive(false);
        // _light.enabled = false;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        
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

        NoiseManager.Instance.RaiseNoise(transform.position);
        AudioManager.Instance.PlayRandomBreak(materialType);

        // Play preloaded noise particles without delay
        // if (noiseParticles != null)
        // {
        //     noiseParticles.transform.position = transform.position;
        //     noiseParticles.gameObject.SetActive(true);
        //
        //     // ensure no old delay or particles remain
        //     noiseParticles.Clear(true);
        //     noiseParticles.Play();
        // }

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

            NoiseManager.Instance.RaiseNoise(other.transform.position);
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

        // // Reset noise particle state
        // if (noiseParticles != null)
        // {
        //     noiseParticles.Stop();
        //     noiseParticles.gameObject.SetActive(false);
        // }
    }

    public void turnOfParticles()
    {
        print(" particles turned off");
        // if (noiseParticles != null)
        // {
        //     noiseParticles.Clear(true);
        //     noiseParticles.gameObject.SetActive(false);
        //     noiseParticles.Stop();
        //     
        // }
    }
    
    
    
    public void BreakObject()
    {
        var ef = GameObject.FindObjectOfType<ExplosionForce>();
        if (ef != null)
            ef.doExplosion(transform.position);
    }
}
