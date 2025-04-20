using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack: MonoBehaviour
{

    [SerializeField] private int attackPower;
    [SerializeField] private int superAttackPower;
    
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
        Debug.Log("attack");
    }

    private void superAttack()
    {
        Debug.Log("superattack");
        superAttacked = false;
    }
}