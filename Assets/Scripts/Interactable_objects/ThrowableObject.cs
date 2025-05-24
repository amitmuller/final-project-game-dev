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
    
    public void grab()
    {
        indicatorInstance.SetActive(false);
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

