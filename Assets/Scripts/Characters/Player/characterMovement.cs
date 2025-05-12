using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

//This script handles moving the character on the X axis, both on the ground and in the air.

public class characterMovement : MonoBehaviour
{
     [Header("Components")]
    private Rigidbody2D body;
    characterGround ground;

    [Header("Movement Stats")]
    [SerializeField, Range(0f, 20f)][Tooltip("Maximum movement speed")] public float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed")] public float maxAcceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop after letting go")] public float maxDecceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop when changing direction")] public float maxTurnSpeed = 80f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed when in mid-air")] public float maxAirAcceleration;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop in mid-air when no direction is used")] public float maxAirDeceleration;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop when changing direction when in mid-air")] public float maxAirTurnSpeed = 80f;
    [SerializeField][Tooltip("Friction to apply against movement on stick")] private float friction;
    

    private bool canMove;

    // [Header("Options")]
    // [Tooltip("When false, the charcter will skip acceleration and deceleration and instantly move and stop")] public bool useAcceleration;
    // public bool itsTheIntro = true;

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
    
    private float size;
    private void Awake()
    {
        //Find the character's Rigidbody and ground detection script
        body = GetComponent<Rigidbody2D>();
        ground = GetComponent<characterGround>();
        size = transform.localScale.x;
    }

    private void OnDisable()
    {
        directionX = 0;
        body.linearVelocity = Vector2.zero;
        canMove = false;
    }

    private void OnEnable()
    {
        canMove = true;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            directionX = context.ReadValue<Vector2>().x;

        }
        
        //This is called when you input a direction on a valid input type, such as arrow keys or analogue stick
        //The value will read -1 when pressing left, 0 when idle, and 1 when pressing right.
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (canDash)
        {
            Debug.Log("in dash");
            StartCoroutine(dash());
            
        }
        //This is called when you input a direction on a valid input type, such as arrow keys or analogue stick
        //The value will read -1 when pressing left, 0 when idle, and 1 when pressing right.
    }
    
    public Vector3 GetPosition() {
        return transform.position;
    }

    private void Update()
    {
        //Used to stop movement when the character is playing her death animation
        if (isDashing)
        {
            return;
        }

        //Used to flip the character's sprite when she changes direction
        //Also tells us that we are currently pressing a direction button
        if (directionX != 0)
        {
            transform.localScale = new Vector3(directionX > 0 ? size : -size, size, size);
            pressingKey = true;
        }
        else
        {
            pressingKey = false;
        }

        //Calculate's the character's desired velocity - which is the direction you are facing, multiplied by the character's maximum speed
        //Friction is not used in this game
        desiredVelocity =canMove? new Vector2(directionX, 0f) * Mathf.Max(maxSpeed - friction, 0f): Vector2.zero;

    }

    private void FixedUpdate()
    {
        //Fixed update runs in sync with Unity's physics engine
        if (isDashing)
        {
            return;
        }

        //Get Kit's current ground status from her ground script
        // onGround = ground.GetOnGround();

        //Get the Rigidbody's current velocity
        velocity = body.linearVelocity;
        if (canMove)
        {
            move();
        }

        
        
        //Calculate movement, depending on whether "Instant Movement" has been checked
        // if (useAcceleration)
        // {
        //     runWithAcceleration();
        // }
        // else
        // {
        //     if (onGround)
        //     {
        //         runWithoutAcceleration();
        //     }
        //     else
        //     {
        //         runWithAcceleration();
        //     }
        // }
        
        
        // if (onGround)
        // {
        //     body.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        // }
        // else
        // {
        //     body.constraints = RigidbodyConstraints2D.FreezeRotation;
        // }
    }
    


    private void runWithAcceleration()
    {
        //Set our acceleration, deceleration, and turn speed stats, based on whether we're on the ground on in the air

        acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        deceleration = onGround ? maxDecceleration : maxAirDeceleration;
        turnSpeed = onGround ? maxTurnSpeed : maxAirTurnSpeed;
        
        if (pressingKey)
        {
            //If the sign (i.e. positive or negative) of our input direction doesn't match our movement, it means we're turning around and so should use the turn speed stat.
            if (Mathf.Sign(directionX) != Mathf.Sign(velocity.x))
            {
                maxSpeedChange = turnSpeed * Time.fixedDeltaTime;
            }
            else
            {
                //If they match, it means we're simply running along and so should use the acceleration stat
                maxSpeedChange = acceleration * Time.fixedDeltaTime;
            }
        }
        else
        {
            //And if we're not pressing a direction at all, use the deceleration stat
            maxSpeedChange = deceleration * Time.fixedDeltaTime;
        }
        

        //Move our velocity towards the desired velocity, at the rate of the number calculated above
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        //Update the Rigidbody with this new velocity
        velocity.y = body.linearVelocity.y;
        body.linearVelocity = velocity;

    }

    private void move()
    {
        //If we're not using acceleration and deceleration, just send our desired velocity (direction * max speed) to the Rigidbody
        velocity.x = desiredVelocity.x;
        // velocity.y = body.linearVelocity.y;
        body.linearVelocity = velocity;
    }

    private IEnumerator dash()
    {
        Debug.Log("in dash");
        Debug.Log(desiredVelocity);
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
        // if (context.performed && canChangeAxis)
        // {
        //     directionY = context.ReadValue<Vector2>().y;
        //     Debug.Log("changeAxis() "+indexList[currentAxis]);
        //     Debug.Log(currentAxis);
        //     
        // }

        
        
        //This is called when you input a direction on a valid input type, such as arrow keys or analogue stick
        //The value will read -1 when pressing left, 0 when idle, and 1 when pressing right.
    }
    
    



    



}