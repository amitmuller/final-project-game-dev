using UnityEngine;

public class CartTrigger : MonoBehaviour
{
    [Tooltip("Index into GameManager.carts for this cart (0-based).")]
    public int cartIndex = 0;

    private void Reset()
    {
        Collider2D col2d = GetComponent<Collider2D>();
        if (col2d != null)
            col2d.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("CartTrigger.OnTriggerEnter2D: No GameManager.Instance in the scene!");
            return;
        }

        GameManager.Instance.PlayerEnteredCart(cartIndex);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance == null) return;
        GameManager.Instance.PlayerLeftCart(cartIndex);
    }
}