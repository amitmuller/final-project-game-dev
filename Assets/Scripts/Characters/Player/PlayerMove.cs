using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D _rb;
    private characterGround groundCheck;
    public LayerMask groundLayer;
    private Vector2 _moveInput;

    private float horizontal;
    private float speed = 8f;
    private bool isFacingRight = true;
    
    private bool canMove;

    [Header("Jump Tweaks")]
    [Range(0f, 18f)]
    public float jumpingPower = 8f; 
    [Range(0f, 5f)]public  float fallMultiplier = 2.5f;
    [Range(0f, 5f)]public float lowJumpMultiplier = 2f;
    [Range(0f, 10f)]public float jumpHeight = 5f;
    private bool desiredJump;
    private bool pressingJump;
    
    private float originalY;
    private bool isJumping = false;

    private bool jumpHeld = false;
    
    [Header("Dash Settings")]
    [SerializeField] private int dashSpeed = 20;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashCoolDown = 1f;
    [SerializeField] private TrailRenderer tr;
    private bool isDashing;
    private bool canDash = true;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<characterGround>();
    }

    void Update()
    {
        // Flip the sprite if needed
        if (isDashing)
        {
            return;
        }
        
        if (!isFacingRight && _moveInput.x > 0f)
        {
            Flip();
        }
        else if (isFacingRight && _moveInput.x  < 0f)
        {
            Flip();
        }
        

        // // Apply gravity tweaks
        // if (
        //     _rb.linearVelocity.y < 0)
        // {
        //     _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        // }
        // else if (_rb.linearVelocity.y > 0 && !jumpHeld)
        // {
        //     _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        // }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        Move();
    }

    public void onMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
    
    private void Move()
    {
        Vector2 previousPosition = _rb.position;
        Vector2 movement = _rb.position + _moveInput * (speed * Time.fixedDeltaTime);
        _rb.MovePosition(movement);
        
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (canDash)
        {
            StartCoroutine(dash());
        }
    }

    public void onJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            desiredJump = true;
            pressingJump = true;
        }

        if (context.canceled)
        {
            pressingJump = false;
        }
    }

    private void DoAJump()
    {
        if (IsGrounded())
        {
            isJumping = true;
            originalY = transform.position.y;
            jumpingPower = Mathf.Sqrt(-2f * Physics2D.gravity.y * _rb.gravityScale * jumpHeight);
            _moveInput.y += jumpingPower;
            isJumping = true;
            
        }
    }
    
    private IEnumerator dash()
    {
        Debug.Log("in dash");
        Debug.Log(_moveInput);
        canDash = false;
        isDashing = true;
        float originalGravity = _rb.gravityScale;
        _rb.gravityScale = 0f;
        _rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed,transform.localScale.y);
        tr.emitting = true;
        yield return new WaitForSeconds(dashDuration);
        tr.emitting = false;
        _rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }

    private bool IsGrounded()
    {
        return groundCheck.GetOnGround();
    }

    public bool isInDash()
    {
        return isDashing;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}