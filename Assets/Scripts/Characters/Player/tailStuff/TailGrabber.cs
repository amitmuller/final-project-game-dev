using System.Collections;
using Characters.Player;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class TailGrabber : MonoBehaviour
{
    private Rigidbody2D heldObject = null;
    
    private float holdStartTime;
    private bool isHolding;
    
    [Header("Grabber")]
    [SerializeField]private TailConnector connector;

    [Header("Throw Settings")]
    [Range(0.5f, 5f)] public float maxChargeTime = 2f;
    [Range(0f, 10f)]      public float minThrowForce = 5f;
    [Range(10f, 50f)]     public float maxThrowForce = 25f;
    [Range(0f, 0.5f)]     public float releaseDelay = 0.15f;

    [Header("Charge Timing")]
    [Tooltip("How long (in seconds) to pause at min/max before reversing")]
    public float edgeHoldTime = 0.2f;

    [Header("Trajectory Preview")]
    public int    trajectoryPoints   = 30;
    public float  timeBetweenPoints  = 0.1f;
    public float  verticalThrowAngle = 1.5f;
    public float  lineZOffset        = -1f;
    public float  maxLineLength      = 4f;
    public float  floorY      = 2f;
    public Gradient aimGradient;

    [Header("References")]
    [SerializeField] private PlayerHide playerHide;

    [Header("Impact Marker")]
    [SerializeField] private GameObject impactMarkerPrefab;
    private GameObject impactMarkerInstance;

    private LineRenderer    aimLine;
    private characterAnimation anim;
    private Transform       initialParent;
    private Coroutine       delayedThrowCoroutine;

    void Awake()
    {
        anim          = GetComponentInParent<characterAnimation>();
        initialParent = transform.parent;

        if (impactMarkerPrefab != null)
        {
            impactMarkerInstance = Instantiate(impactMarkerPrefab);
            impactMarkerInstance.SetActive(false);
        }

        aimLine = GetComponent<LineRenderer>();
        aimLine.positionCount   = 2;
        aimLine.enabled         = false;
        aimLine.material        = new Material(Shader.Find("Sprites/Default"));
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
                isHolding     = true;
                aimLine.enabled = true;
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
            float elapsed = Time.time - holdStartTime;
            float cycle   = 2f * maxChargeTime + 2f * edgeHoldTime;
            float m       = elapsed % cycle;

            float t;
            if (m < edgeHoldTime)
            {
                t = 0f;
            }
            else if (m < edgeHoldTime + maxChargeTime)
            {
                t = (m - edgeHoldTime) / maxChargeTime;
            }
            else if (m < edgeHoldTime + maxChargeTime + edgeHoldTime)
            {
                t = 1f;
            }
            else
            {
                t = 1f - (m - (edgeHoldTime + maxChargeTime + edgeHoldTime)) / maxChargeTime;
            }

            float force = Mathf.Lerp(minThrowForce, maxThrowForce, t);

            delayedThrowCoroutine = StartCoroutine(DelayedThrow(force));
            anim.TransitionTo(PlayerAnimState.TailThrow);
            heldObject.GetComponent<Collider2D>().isTrigger = false;
            isHolding = false;
            aimLine.enabled = false;
        }
    }

    public void Grab()
    {
        if (heldObject != null && !connector.IsConnected)
        {
            connector.Attach(heldObject);
            heldObject.GetComponent<ThrowableObject>()?.GrabObject();
            
            

            var playerRenderer = GetComponentInParent<Renderer>();
            var objRenderer    = heldObject.GetComponent<Renderer>();
            if (playerRenderer != null && objRenderer != null)
                playerHide.UpdateHeldObjectSorting();
        }
    }

    private IEnumerator DelayedThrow(float force)
    {
        yield return new WaitForSeconds(releaseDelay);

        if (connector.IsConnected)
        {
            float facing  = Mathf.Sign(transform.lossyScale.x);
            Vector2 dir   = new Vector2(-facing, verticalThrowAngle).normalized;

            connector.Detach();
            impactMarkerInstance.SetActive(false);

            heldObject.isKinematic = false;
            heldObject.AddForce(dir * force, ForceMode2D.Impulse);
            heldObject = null;
            delayedThrowCoroutine = null;
        }
    }

    public bool HasObject => heldObject != null;

    void Update()
    {
        if (!isHolding) return;

        float elapsed = Time.time - holdStartTime;
        float cycle   = 2f * maxChargeTime + 2f * edgeHoldTime;
        float m       = elapsed % cycle;

        float t;
        if (m < edgeHoldTime)
        {
            t = 0f;
        }
        else if (m < edgeHoldTime + maxChargeTime)
        {
            t = (m - edgeHoldTime) / maxChargeTime;
        }
        else if (m < edgeHoldTime + maxChargeTime + edgeHoldTime)
        {
            t = 1f;
        }
        else
        {
            t = 1f - (m - (edgeHoldTime + maxChargeTime + edgeHoldTime)) / maxChargeTime;
        }

        float force = Mathf.Lerp(minThrowForce, maxThrowForce, t);
        DrawTrajectory(force);
    }

    public Renderer GetHeldObjectRenderer()
    {
        return heldObject != null ? heldObject.GetComponent<Renderer>() : null;
    }

    private void DrawTrajectory(float force)
    {
        if (heldObject == null) return;

        Vector3 startPos = heldObject.transform.position;
        Vector2 velocity = new Vector2(-Mathf.Sign(transform.lossyScale.x), verticalThrowAngle)
                            .normalized * force;
        Vector2 gravity  = Physics2D.gravity;

        Vector3[] points = new Vector3[trajectoryPoints];
        points[0] = startPos;
        aimLine.sortingOrder = 11;

        for (int i = 1; i < trajectoryPoints; i++)
        {
            float time = i * timeBetweenPoints;
            Vector2 disp2D = velocity * time + 0.5f * gravity * time * time;
            Vector3 nextPos = startPos + (Vector3)disp2D;

            Vector3 prevPos = points[i - 1];
            if (i > 10)
            {
                ContactFilter2D filter = new ContactFilter2D { useTriggers = false, useLayerMask = false };
                RaycastHit2D[] hits = new RaycastHit2D[1];
                int count = Physics2D.Linecast(prevPos, nextPos, filter, hits);

                if (count > 0)
                {
                    var hit = hits[0];
                    if (hit.collider != null && hit.collider.attachedRigidbody != heldObject)
                    {
                        Vector3 hitPoint3 = hit.point;
                        points[i] = hitPoint3;
                        if (impactMarkerInstance != null)
                        {
                            impactMarkerInstance.SetActive(true);
                            impactMarkerInstance.transform.position = hitPoint3;
                        }
                        aimLine.positionCount = i + 1;
                        aimLine.SetPositions(points);
                        return;
                    }
                }            }
            

            points[i] = nextPos;
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
        if (delayedThrowCoroutine != null)
        {
            StopCoroutine(delayedThrowCoroutine);
            delayedThrowCoroutine = null;
        }

        isHolding = false;
        aimLine.enabled = false;

        if (connector.IsConnected)
            connector.reset();

        heldObject = null;

        if (impactMarkerInstance != null)
            impactMarkerInstance.SetActive(false);
    }
}
