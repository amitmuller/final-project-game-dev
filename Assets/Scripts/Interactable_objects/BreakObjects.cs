using UnityEngine;

public class BreakObjects : MonoBehaviour
{
    [SerializeField] private GameObject breakObjects;
    private Explodable explodable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        explodable = GetComponent<Explodable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BreakObject()
    {
        Instantiate(breakObjects, transform.position, Quaternion.identity);
        explodable.explode();
        gameObject.SetActive(false);
        ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
        ef.doExplosion(transform.position);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if (!collision.gameObject.CompareTag("Player")) return;
        // Instantiate(breakObjects, transform.position, Quaternion.identity);
        // gameObject.SetActive(false);
    }
}