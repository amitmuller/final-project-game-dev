using UnityEngine;

public class BreakObjects : MonoBehaviour
{
    [SerializeField] private GameObject breakObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        Instantiate(breakObjects, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
