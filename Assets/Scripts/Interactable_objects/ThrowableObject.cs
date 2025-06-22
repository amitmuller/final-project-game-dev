using System;
using Light;
using UnityEditor.Experimental.GraphView;
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
    private GameObject initialParent;
    

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        noiseParticles = GetComponentInChildren<ParticleSystem>();
        originalColor = sr.color;
        var prefab = Resources.Load<GameObject>("takeThrowIcon");
        if (prefab)
        {
            indicatorInstance = Instantiate(prefab, transform);
            indicatorInstance.transform.localPosition = new Vector3(0, 1f, 0); // adjust offset as needed
            indicatorInstance.SetActive(false);
        }
        else
        {
            Debug.LogError("Couldn't load hideIcon prefab from Resources", this);
        }
        initialPosition = transform.position;
        originalLayer = gameObject.layer;
        initialParent = transform.parent.gameObject;

    }

    public void Highlight(bool enable)
    {
        sr.color = enable ? highlightColor : originalColor;
        if (indicatorInstance != null)
        {
            indicatorInstance.SetActive(enable);
        }
    }
    
    public void GrabObject()
    {
        indicatorInstance.SetActive(false);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // You can filter with collision.gameObject.tag if needed
        if (collision.gameObject.CompareTag("lightBolb"))
        {
            var lamp = collision.gameObject.GetComponent<LighBulb>();
            if (lamp != null)
            {
                lamp.AlertNearbyEnemies();
            }

            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("ground"))
        {
            
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
        NoiseManager.RaiseNoise(transform.position);
        noiseParticles.gameObject.transform.position = transform.position;
        noiseParticles.Play();
        gameObject.layer = LayerMask.NameToLayer("notCollide");
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 11;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.CompareTag("lightBolb"))
        {
            
            var lamp = other.gameObject.GetComponent<LighBulb>();
            if (lamp != null)
            {
                lamp.AlertNearbyEnemies();
            }
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
        gameObject.transform.SetParent(initialParent.transform);
        gameObject.transform.position = initialPosition;
        gameObject.layer = originalLayer;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        GetComponent<Rigidbody2D>().isKinematic = true;
    }
}

