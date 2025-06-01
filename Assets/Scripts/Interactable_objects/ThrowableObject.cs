using Light;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;
    public Color highlightColor = Color.yellow;
    private GameObject indicatorInstance;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
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
            NoiseManager.RaiseNoise(transform.position);
            gameObject.layer = LayerMask.NameToLayer("notCollide");
        }

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
        NoiseManager.RaiseNoise(transform.position);
        
    }
}

