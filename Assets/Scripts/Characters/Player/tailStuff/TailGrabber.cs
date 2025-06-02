using System.Collections;
using Characters.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class TailGrabber : MonoBehaviour
{
    private Rigidbody2D heldObject = null;
    private TailConnector connector;
    private float holdStartTime;
    private bool isHolding;
    private bool ishiding = false;

    [Range(0.5f, 5f)] public float maxChargeTime = 2f;
    [Range(0f, 10f)] public float minThrowForce = 5f;
    [Range(10f, 50f)] public float maxThrowForce = 25f;
    [Range(0f, 0.5f)] public float releaseDelay = 0.15f;
    [SerializeField] private PlayerHide playerHide;

    void Awake()
    {
        connector = GetComponent<TailConnector>();
        playerHide = GetComponentInParent<PlayerHide>();
        
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Throwable") && heldObject == null && !playerHide.IsHiding())
        {
            heldObject = other.attachedRigidbody;
            Debug.Log(heldObject.name+ "inTrigger");
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
                Debug.Log("Grab"+ heldObject);
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
            Debug.Log(heldObject.gameObject);
            StartCoroutine(DelayedThrow(force));
            heldObject.gameObject.GetComponent<Collider2D>().isTrigger = false;
            isHolding = false;
        }
    }

    public void Grab()
    {
        if (heldObject != null && !connector.IsConnected)
        {
            Debug.Log("inGrab");
            connector.Attach(heldObject);
            Debug.Log("Grab"+ heldObject.GetComponent<ThrowableObject>());
            heldObject.GetComponent<ThrowableObject>()?.GrabObject();
            
            
        }
    }

    private IEnumerator DelayedThrow(float force)
    {
        yield return new WaitForSeconds(releaseDelay);

        if (connector.IsConnected)
        {
            float facing = Mathf.Sign(transform.lossyScale.x); // +1 right, -1 left
            Vector2 throwDir = new Vector2(-facing, 0f).normalized;
            Vector2 baseDir = new Vector2(-facing , 1.5f);
            connector.Detach();
            heldObject.isKinematic = false;
            heldObject.AddForce(baseDir * force, ForceMode2D.Impulse);
            heldObject = null;
        }
    }

    public bool HasObject => heldObject != null;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 2f);
    }
}
