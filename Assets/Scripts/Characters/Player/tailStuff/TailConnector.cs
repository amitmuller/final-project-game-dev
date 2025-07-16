using Unity.VisualScripting;
using UnityEngine;

public class TailConnector : MonoBehaviour
{
    private Transform heldObjectTransform;
    private Rigidbody2D heldRigidbody;

    public void Attach(Rigidbody2D target)
    {
        heldObjectTransform = target.transform;
        heldRigidbody = target;
        // Disable physics so object follows tail exactly
        heldRigidbody.isKinematic = true;
        heldObjectTransform.SetParent(transform);
        // heldObjectTransform.localPosition = new Vector3(0,0,0); // Snap to tail
        heldObjectTransform.localPosition = new Vector3(0f,0f,0); // Snap to tail
    }

    public void Detach()
    {
        if (heldObjectTransform != null)
        {
            heldObjectTransform.SetParent(null);
            heldRigidbody.isKinematic = false;

            heldObjectTransform = null;
            heldRigidbody = null;
        }
    }

    public void reset()
    {
        heldObjectTransform.GetComponent<ThrowableObject>().turnOfParticles();
        Destroy(heldObjectTransform.gameObject);
        heldObjectTransform = null;
    }

    public bool IsConnected => heldObjectTransform != null;
}