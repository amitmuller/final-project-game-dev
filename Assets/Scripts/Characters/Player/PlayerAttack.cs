using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack: MonoBehaviour
{

    [SerializeField] private int attackPower;
    [SerializeField] private int superAttackPower;
    [SerializeField] private float timeOfAttack = 0.3f;
    [SerializeField] private float attackRadius = 1f;
    private bool isAttacking = false;
    
    
    private Rigidbody2D _rb;
    private bool superAttacked = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void onAttack(InputAction.CallbackContext context)
    {
        if (context.performed && _rb.linearVelocity != Vector2.zero)
        {
            superAttacked = true;
            superAttack();
        }
        if (context.performed && _rb.linearVelocity == Vector2.zero && !superAttacked) attack();
    }
    

    private void attack()
    {
        isAttacking = true;
        Invoke("notAttacking", timeOfAttack);
        Debug.Log("attack");

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius); // Adjust radius as needed
        Debug.Log(hits.Length);
        foreach (var hit in hits)
        {
            Debug.Log("hit", hit.gameObject);
            if (hit.CompareTag("breakableObject"))
            {
                Debug.Log("Broke object during attack");
                hit.GetComponent<BreakObjects>()?.BreakObject();
            }
        }
    }


    void notAttacking()
    {
        isAttacking = false;
        superAttacked = false;
    }

    private void superAttack()
    {
        Debug.Log("superattack");
        Invoke("notAttacking", timeOfAttack);
        isAttacking = true;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius); // Adjust radius as needed
        foreach (var hit in hits)
        {
            Debug.Log("hit", hit.gameObject);
            if (hit.CompareTag("breakableObject"))
            {
                Debug.Log("Broke object during attack");
                hit.GetComponent<BreakObjects>()?.BreakObject();
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius); // Match the radius in attack()
    }
    

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     
    //     if (other.gameObject.CompareTag("breakableObject"))
    //     {
    //         Debug.Log("Collision");
    //         other.gameObject.GetComponent<BreakObjects>().BreakObject();
    //         notAttacking();
    //     }
    // }
}