using System;
using System.Collections;
using Characters.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Spine.Unity;
using Spine.Unity.Examples;

public class characterMovement : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D body;
    characterGround ground;

    [FormerlySerializedAs("maxSpeed")]
    [Header("Movement Stats")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float maxAcceleration = 52f;
    [SerializeField] private float maxDecceleration = 52f;
    [SerializeField] private float maxTurnSpeed = 80f;
    [SerializeField] private float maxAirAcceleration;
    [SerializeField] private float maxAirDeceleration;
    [SerializeField] private float maxAirTurnSpeed = 80f;
    [SerializeField] private float friction;

    private bool canMove;

    [Header("Calculations")]
    public float directionX;
    private Vector2 desiredVelocity;
    public Vector2 velocity;
    private float maxSpeedChange;
    private float acceleration;
    private float deceleration;
    private float turnSpeed;
    private Vector2 _input;

    [Header("Current State")]
    public bool onGround;
    public bool pressingKey;

    [Header("Dash Settings")]
    [SerializeField] private int dashSpeed = 20;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashCoolDown = 1f;
    [SerializeField] private TrailRenderer tr;
    private bool isDashing;
    private bool canDash = true;

    [Header("Slowing Settings")]
    private bool isSlowed = false;
    public float slowFactor = 0.5f;
    public float slowDuration = 2f;

    [Header("Shooting Settings")]
    private Vector2 aimDirection;
    private bool isHoldingAim;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private float aimLineLength = 5f;
    
    [Header("raise noise Settings")]
    [SerializeField] private float noiseLevelToAdd = 0.1f;
    [SerializeField] private float noiseTriggerSpeed = 4f;
    
    [Header("Animation Settings")]
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, walking;
    public string currentAnimationName;
    
    private float size;
    private Vector2 rawMoveInput;
    

    private void Awake()
    {
        currentAnimationName = "idle";
        SetCharacterState(currentAnimationName);
        body = GetComponent<Rigidbody2D>();
        ground = GetComponent<characterGround>();
        size = transform.localScale.x;

        if (aimLine != null)
        {
            aimLine.positionCount = 2;
            aimLine.enabled = false;
        }
    }

    private void OnEnable() => canMove = true;

    private void OnDisable()
    {
        directionX = 0;
        body.linearVelocity = Vector2.zero;
        canMove = false;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        if (isHoldingAim)
        {
            aimDirection = _input;
        }
        else if (canMove)
        { 
            directionX = _input.x;
            rawMoveInput = _input;
            
        }
        
    }

    public void OnSpit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHoldingAim = true;
            canMove = false;
            if (aimLine != null) aimLine.enabled = true;
        }

        if (context.canceled)
        {
            isHoldingAim = false;
            canMove = true;
            directionX = 0f;

            if (aimLine != null) aimLine.enabled = false;

            Shoot(aimDirection);
            aimDirection = Vector2.zero;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (canDash)
        {
            StartCoroutine(dash());
        }
    }

    public Vector3 GetPosition() => transform.position;

    private void Update()
    {
        
        if (isDashing) return;

        
        float horizontalSpeed = Mathf.Abs(body.linearVelocity.x);
        // noiseTimer -= Time.deltaTime;
        if (horizontalSpeed >= noiseTriggerSpeed)
        {
            NoiseUIManager.Instance?.AddNoise(noiseLevelToAdd); // Add normalized noise level
        }

        
        if (directionX != 0)
        {
            transform.localScale = new Vector3(directionX > 0 ? size : -size, size, size);
            pressingKey = true;
        }
        else
        {
            pressingKey = false;
        }


        desiredVelocity = canMove ?  
            Vector2.Lerp(desiredVelocity, new Vector2(rawMoveInput.x, 0f) * maxSpeed, Time.deltaTime * 10f) 
            : Vector2.zero;
        if (desiredVelocity == Vector2.zero)
        {
            Debug.Log("Can't move");
            SetCharacterState("idle");
        }
        else if (desiredVelocity != Vector2.zero)
        {
            Debug.Log("CAN MOVE");
            SetCharacterState("walking");
        }
        

        // Draw aim line
        if (isHoldingAim && aimLine != null && aimDirection != Vector2.zero)
        {
            Vector3 start = firePoint.position;
            Vector3 end = start + (Vector3)(aimDirection.normalized * aimLineLength);
            aimLine.SetPosition(0, start);
            aimLine.SetPosition(1, end);
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        velocity = body.linearVelocity;

        if (canMove || _input.y <= 0.8f)
        {
            move();
        }
    }

    private void move()
    {
        if (Mathf.Abs(directionX) > 0.01f)
        {
            // Apply desired movement
            velocity.x = desiredVelocity.x;
        }
        else
        {
            // Apply friction to stop sliding
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, friction * Time.fixedDeltaTime);
        }

        body.linearVelocity = velocity;
    }

    private void Shoot(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.1f)
        {
            return;
        }

        var projectileGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        var projectile = projectileGO.GetComponent<Characters.Player.PlayerProjectile>();
        projectile.SetDirection(direction);
    }

    private IEnumerator dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = body.gravityScale;
        body.gravityScale = 0f;
        body.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashDuration);
        tr.emitting = false;
        body.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }

    public void ApplySlow()
    {
        if (!isSlowed)
            StartCoroutine(SlowCoroutine());
    }

    private IEnumerator SlowCoroutine()
    {
        isSlowed = true;
        float originalSpeed = speed;
        speed *= slowFactor;

        yield return new WaitForSeconds(slowDuration);

        speed = originalSpeed;
        isSlowed = false;
    }
    public Vector2 MoveInput => _input;

    public void SetCanMove(bool move)
    {
        canMove = move;

        if (!canMove)
        {
            directionX = 0;
            rawMoveInput = Vector2.zero;
            desiredVelocity = Vector2.zero;
            velocity = Vector2.zero;
            body.linearVelocity = Vector2.zero;
        }
    }

    public void SetAnimation(AnimationReferenceAsset animation, bool loop)
    {
        if (skeletonAnimation == null || animation == null)
            return;

        if (currentAnimationName == animation.name)
            return; // Avoid restarting same animation

        skeletonAnimation.state.SetAnimation(0, animation, loop);
        currentAnimationName = animation.name;
    }

    public void SetCharacterState(string state)
    {
        if (state.Equals("idle"))
        {
            SetAnimation(idle, true);
        }
        else if (state.Equals("walking"))
        {
            SetAnimation(walking, true);
        }
    }
}
