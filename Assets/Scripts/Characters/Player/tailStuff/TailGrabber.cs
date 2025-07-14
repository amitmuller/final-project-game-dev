using System.Collections;
using Characters.Player;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(TailConnector))]
public class TailGrabber : MonoBehaviour
{
    private Rigidbody2D heldObject = null;
    private TailConnector connector;
    private float holdStartTime;
    private bool isHolding;

    [Header("Throw Settings")]
    [Range(0.5f, 5f)] public float maxChargeTime = 2f;
    [Range(0f, 10f)] public float minThrowForce = 5f;
    [Range(10f, 50f)] public float maxThrowForce = 25f;
    [Range(0f, 0.5f)] public float releaseDelay = 0.15f;

    [Header("Trajectory Preview")]
    public int trajectoryPoints = 30;
    public float timeBetweenPoints = 0.1f;
    public float verticalThrowAngle = 1.5f;
    public float lineZOffset = -1f;
    public float maxLineLength = 4f;
    public Gradient aimGradient;

    [Header("References")]
    [SerializeField] private PlayerHide playerHide;
    
    [Header("Impact Marker")]
    [SerializeField] private GameObject impactMarkerPrefab;
    private GameObject impactMarkerInstance;
    

    private LineRenderer aimLine;
    private characterAnimation  anim;
    private Transform initialParent;
    private Coroutine delayedThrowCoroutine;

    void Awake()
    {
        connector = GetComponent<TailConnector>();
        anim      = GetComponentInParent<characterAnimation>();
        initialParent = transform.parent;
        if (impactMarkerPrefab != null)
        {
            impactMarkerInstance = Instantiate(impactMarkerPrefab);
            impactMarkerInstance.SetActive(false);
        }
        
        aimLine = GetComponent<LineRenderer>();
        aimLine.positionCount = 2;
        aimLine.enabled = false;
        aimLine.material = new Material(Shader.Find("Sprites/Default"));
        aimLine.widthMultiplier = 0.05f;

        if (aimGradient != null)
            aimLine.colorGradient = aimGradient;
    }
    private void OnEnable()
    {
        GameManager.OnPlayerDead += HandlePlayerDead;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerDead -= HandlePlayerDead;
    }

    private void HandlePlayerDead()
    {
        ResetGrabber();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Throwable") && heldObject == null)
        {
            heldObject = other.attachedRigidbody;
            other.GetComponent<ThrowableObject>()?.Highlight(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody == heldObject)
        {
            other.GetComponent<ThrowableObject>()?.Highlight(false);
            heldObject = null;
        }
    }

    public void onGrab(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (connector.IsConnected)
            {
                holdStartTime = Time.time;
                isHolding = true;
                aimLine.enabled = true;
                // transform.parent = 
                anim.TransitionTo(PlayerAnimState.TailAim);
            }
            else if (heldObject != null)
            {
                var objectToGrab = heldObject;
                anim.TransitionTo(PlayerAnimState.TailPick, entry =>
                {
                    heldObject = objectToGrab;
                    Grab();
                });
                
            }
        }
        else if (context.canceled && isHolding)
        {
            float chargeTime = Time.time - holdStartTime;
            float force = Mathf.Lerp(minThrowForce, maxThrowForce, Mathf.Clamp01(chargeTime / maxChargeTime));
            delayedThrowCoroutine = StartCoroutine(DelayedThrow(force));
            anim.TransitionTo(PlayerAnimState.TailThrow);
            heldObject.GetComponent<Collider2D>().isTrigger = false;
            isHolding = false;
            aimLine.enabled = false;
        }
    }

    public void Grab()
    {
        print("grab");
        if (heldObject != null && !connector.IsConnected)
        {
            
            connector.Attach(heldObject);
            heldObject.GetComponent<ThrowableObject>()?.GrabObject();
            var playerRenderer = GetComponentInParent<Renderer>();
            
            var objRenderer = heldObject.GetComponent<Renderer>();
            if (playerRenderer != null && objRenderer != null)
            {

                playerHide.UpdateHeldObjectSorting();
            }
        }
    }

    private IEnumerator DelayedThrow(float force)
    {
        yield return new WaitForSeconds(releaseDelay);

        if (connector.IsConnected)
        {
            float facing = Mathf.Sign(transform.lossyScale.x); // +1 right, -1 left
            Vector2 throwDir = new Vector2(-facing, verticalThrowAngle).normalized;

            connector.Detach();
            impactMarkerInstance.SetActive(false);
            
            heldObject.isKinematic = false;
            heldObject.AddForce(throwDir * force, ForceMode2D.Impulse);
            heldObject = null;
            delayedThrowCoroutine = null;
        }
    }

    public bool HasObject => heldObject != null;

    void Update()
    {
        if (!isHolding) return;

        float chargeTime = Time.time - holdStartTime;
        float t = Mathf.Clamp01(chargeTime / maxChargeTime);
        float force = Mathf.Lerp(minThrowForce, maxThrowForce, t);

        DrawTrajectory(force);
    }
    public Renderer GetHeldObjectRenderer()
    {
        return heldObject != null ? heldObject.GetComponent<Renderer>() : null;
    }

    private void DrawTrajectory(float force)
    {
        float facing = Mathf.Sign(transform.lossyScale.x);
        Vector2 direction = new Vector2(-facing, verticalThrowAngle).normalized;
        Vector2 velocity = direction * force;
        Vector2 gravity = Physics2D.gravity;

        Vector3[] points = new Vector3[trajectoryPoints];

        Vector3 startPos = heldObject.transform.position;
        aimLine.sortingOrder     =   GetHeldObjectRenderer().sortingOrder;

        points[0] = startPos;

        for (int i = 1; i < trajectoryPoints; i++)
        {
            float time = i * timeBetweenPoints;
            Vector2 nextPos = startPos + (Vector3)(velocity * time + 0.5f * gravity * time * time);

            // Check for collision between previous point and next point
            Vector2 prevPos = points[i - 1];
            
            // RaycastHit2D hit = Physics2D.Linecast(prevPos, nextPos, hits); // or use your own layer
            // before your loop, build a filter that ignores triggers
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = false;           // ← don’t hit triggers
            filter.useLayerMask = false;         // ← optional: you can also add layer filtering

            RaycastHit2D[] hits = new RaycastHit2D[1];
            
            int count = Physics2D.Linecast(prevPos, nextPos, filter, hits);
            if (count > 0)
            {
                var hit = hits[0];
                points[i] = hit.point;
                Debug.Log(hit.collider.name);
            
            
            if (hit.collider != null && !(hit.collider.gameObject == heldObject || hit.collider.transform.IsChildOf(heldObject.transform)))
            {
                points[i] = hit.point;
                if (impactMarkerInstance != null)
                {
                    impactMarkerInstance.SetActive(true);
                    impactMarkerInstance.transform.position = hit.point;
                }
                aimLine.positionCount = i + 1;
                aimLine.SetPositions(points);
                return;
            }
            }
            else
            {
                points[i] = nextPos;
            }
        }

        aimLine.positionCount = trajectoryPoints;
        aimLine.SetPositions(points);
    }
    
    /// <summary>
    /// Immediately cancel any grab or throw in progress
    /// and reset the grabber to its initial state.
    /// </summary>
    public void ResetGrabber()
    {
        // 1) stop any pending throw coroutine
        if (delayedThrowCoroutine != null)
        {
            StopCoroutine(delayedThrowCoroutine);
            delayedThrowCoroutine = null;
        }

        // 2) stop aiming preview
        isHolding = false;
        aimLine.enabled = false;

        // 3) call connector.reset() to destroy the held object safely
        if (connector.IsConnected)
        {
            connector.reset();
        }

        // 4) clear our local references
        heldObject = null;

        // 5) hide impact marker
        if (impactMarkerInstance != null)
            impactMarkerInstance.SetActive(false);
        
    }

}
