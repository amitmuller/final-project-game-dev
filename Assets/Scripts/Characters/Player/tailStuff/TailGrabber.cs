using System.Collections;
using Characters.Player;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
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

    void Awake()
    {
        connector = GetComponent<TailConnector>();
        playerHide = GetComponentInParent<PlayerHide>();
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Throwable") && heldObject == null && !playerHide.IsHiding())
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
        if (context.started && !playerHide.IsHiding())
        {
            if (connector.IsConnected)
            {
                holdStartTime = Time.time;
                isHolding = true;
                aimLine.enabled = true;
            }
            else if (heldObject != null)
            {
                Grab();
            }
        }
        else if (context.canceled && isHolding && !playerHide.IsHiding())
        {
            float chargeTime = Time.time - holdStartTime;
            float force = Mathf.Lerp(minThrowForce, maxThrowForce, Mathf.Clamp01(chargeTime / maxChargeTime));

            StartCoroutine(DelayedThrow(force));
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
            heldObject.isKinematic = false;
            heldObject.AddForce(throwDir * force, ForceMode2D.Impulse);
            heldObject = null;
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

    private void DrawTrajectory(float force)
    {
        float facing = Mathf.Sign(transform.lossyScale.x);
        Vector2 direction = new Vector2(-facing, verticalThrowAngle).normalized;
        Vector2 velocity = direction * force;
        Vector2 gravity = Physics2D.gravity;

        Vector3[] points = new Vector3[trajectoryPoints];
        Vector3 startPos = transform.position;

        points[0] = startPos;

        for (int i = 1; i < trajectoryPoints; i++)
        {
            float time = i * timeBetweenPoints;
            Vector2 nextPos = startPos + (Vector3)(velocity * time + 0.5f * gravity * time * time);

            // Check for collision between previous point and next point
            Vector2 prevPos = points[i - 1];
            RaycastHit2D hit = Physics2D.Linecast(prevPos, nextPos, LayerMask.GetMask("Ground")); // or use your own layer

            if (hit.collider != null)
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

            points[i] = nextPos;
        }

        aimLine.positionCount = trajectoryPoints;
        aimLine.SetPositions(points);
    }

}
