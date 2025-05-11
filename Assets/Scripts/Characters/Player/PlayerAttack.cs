using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerAttack: MonoBehaviour
{

    [SerializeField] private int attackPower;
    [SerializeField] private int superAttackPower;
    [SerializeField] private float timeOfAttack = 0.3f;
    [FormerlySerializedAs("attackRadius")] [SerializeField] private float superAttackRadius = 1f;
    [SerializeField] private float attackRadiusFactor = 0.3f;
    private bool isAttacking = false;
    
    
    private Rigidbody2D _rb;
    private PlayerMove _move;
    private bool superAttacked = false;
    private characterGround _ground;
    public bool onGround = true;
    private bool inDash;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ground = GetComponent<characterGround>();
        _move = GetComponent<PlayerMove>();
        foreach (var device in InputSystem.devices)
        {
            Debug.Log("Device: " + device.displayName + " (" + device.name + ")");
        }
    }

    public void onAttack(InputAction.CallbackContext context)
    {
        inDash = _move.isInDash();
        if (context.performed && inDash)
        {
            superAttacked = true;
            superAttack();
        }
        // if (context.performed && onGround && !superAttacked) attack();
        if (context.performed && !superAttacked) attack();
    }
    

    // private void attack()
    // {
    //     isAttacking = true;
    //     Invoke("notAttacking", timeOfAttack);
    //     Debug.Log("attack");
    //
    //     Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius); // Adjust radius as needed
    //     Debug.Log(hits.Length);
    //     foreach (var hit in hits)
    //     {
    //         Debug.Log("hit"+hit.gameObject.name);
    //         if (hit.CompareTag("breakableObject"))
    //         {
    //             Debug.Log("Broke object during attack");
    //             hit.GetComponent<BreakObjects>()?.BreakObject();
    //         }
    //     }
    // }
    
    private Vector2 FacingDirection => transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    
    private void attack()
    {
        isAttacking = true;
        Invoke(nameof(notAttacking), timeOfAttack);
        Debug.Log("attack");

        Vector2 center = (Vector2)transform.position + FacingDirection * (superAttackRadius * 0.7f); // attack in front

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, superAttackRadius * attackRadiusFactor);
        Debug.Log(hits.Length);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("breakableObject"))
            {
                Debug.Log("hit " + hit.gameObject.name);                Debug.Log("Broke object during attack");
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, superAttackRadius); // Adjust radius as needed
        foreach (var hit in hits)
        {
            Debug.Log("hit", hit.gameObject);
            if (hit.CompareTag("breakableObject"))
            {
                Debug.Log("Broke object during attack");
                hit.GetComponent<BreakObjects>()?.BreakObject();
                hit.GetComponent<Rigidbody2D>().AddForce(10 * Vector2.right, ForceMode2D.Impulse);
            }
        }
    }
    
    
    
    private void OnDrawGizmos()
    {
        // Regular attack gizmo - in front of player
        Gizmos.color = Color.red;
        Vector2 regularCenter = (Vector2)transform.position + FacingDirection * (superAttackRadius * 0.7f);
        Gizmos.DrawWireSphere(regularCenter, superAttackRadius * attackRadiusFactor);

        // Super attack gizmo - radial
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, superAttackRadius);
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