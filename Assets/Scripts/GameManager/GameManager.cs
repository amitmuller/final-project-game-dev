using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("each Cart’s data size = number of carts")]
    public List<CartData> carts = new List<CartData>();

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Duplicate GameManager found; destroying the new one.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called by CartTrigger when Player enters Cart with index = cartIndex.
    /// This will activate (SetActive(true)) any enemies that are still off.
    /// </summary>
    public void PlayerEnteredCart(int cartIndex)
    {
        if (cartIndex < 0 || cartIndex >= carts.Count)
        {
            Debug.LogError($"PlayerEnteredCart: invalid index {cartIndex} (must be 0..{carts.Count - 1}).");
            return;
        }

        CartData cart = carts[cartIndex];
        if (!cart.hasActivated)
        {
            ActivateEnemiesInCart(cart);
            cart.hasActivated = true;
            Debug.Log($"[GameManager] Activated enemies for {cart.cartName} (index {cartIndex}).");
        }
        // else: already activated before → do nothing
    }

    private void ActivateEnemiesInCart(CartData cart)
    {
        if (cart.enemies == null || cart.enemies.Count == 0)
        {
            Debug.LogWarning($"Cart '{cart.cartName}' has no enemy references set in the inspector.");
            return;
        }

        foreach (GameObject enemyGO in cart.enemies)
        {
            if (enemyGO == null) continue;
            enemyGO.SetActive(true);
        }
    }
}