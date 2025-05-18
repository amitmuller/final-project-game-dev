using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterMovement : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D body;
    characterGround ground;

    [Header("Movement Stats")]
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

    private float size;

    private void Awake()
    {
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
        Vector2 input = context.ReadValue<Vector2>();

        if (isHoldingAim)
        {
            aimDirection = input;
        }
        else if (canMove)
        {
            directionX = input.x;
        }
    }

    public void OnSpit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHoldingAim = true;
            Debug.Log("Started aiming");
            canMove = false;
            if (aimLine != null) aimLine.enabled = true;
        }

        if (context.canceled)
        {
            isHoldingAim = false;
            Debug.Log("Released aim. Firing at: " + aimDirection);
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
            Debug.Log("in dash");
            StartCoroutine(dash());
        }
    }

    public Vector3 GetPosition() => transform.position;

    private void Update()
    {
        if (isDashing) return;

        if (directionX != 0)
        {
            transform.localScale = new Vector3(directionX > 0 ? size : -size, size, size);
            pressingKey = true;
        }
        else
        {
            pressingKey = false;
        }

        desiredVelocity = canMove ? new Vector2(directionX, 0f) * Mathf.Max(maxSpeed - friction, 0f) : Vector2.zero;

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

        if (canMove)
        {
            move();
        }
    }

    private void move()
    {
        velocity.x = desiredVelocity.x;
        body.linearVelocity = velocity;
    }

    private void Shoot(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.1f)
        {
            Debug.Log("No aim direction — did not shoot.");
            return;
        }

        var projectileGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        var projectile = projectileGO.GetComponent<Characters.Player.PlayerProjectile>();
        projectile.SetDirection(direction);

        Debug.Log("Fired projectile in direction: " + direction.normalized);
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
        float originalSpeed = maxSpeed;
        maxSpeed *= slowFactor;

        yield return new WaitForSeconds(slowDuration);

        maxSpeed = originalSpeed;
        isSlowed = false;
    }

    public void OnChangeAxis(InputAction.CallbackContext context)
    {
        // Reserved for other future inputs
    }
}
