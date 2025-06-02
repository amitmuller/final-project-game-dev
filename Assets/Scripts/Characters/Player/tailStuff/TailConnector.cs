using UnityEngine;

public class TailConnector : MonoBehaviour
{
    private Transform heldObjectTransform;
    private Rigidbody2D heldRigidbody;

    public void Attach(Rigidbody2D target)
    {
        Debug.Log("Attach");
        heldObjectTransform = target.transform;
        heldRigidbody = target;
        Debug.Log("Attached"+heldObjectTransform.name);
        // Disable physics so object follows tail exactly
        heldRigidbody.isKinematic = true;
        heldObjectTransform.SetParent(transform);
        heldObjectTransform.localPosition = Vector3.zero; // Snap to tail
    }

    public void Detach()
    {
        if (heldObjectTransform != null)
        {
            Debug.Log("Detach");

            // Detach from tail
            heldObjectTransform.SetParent(null);
            heldRigidbody.isKinematic = false;

            heldObjectTransform = null;
            heldRigidbody = null;
        }
    }

    public bool IsConnected => heldObjectTransform != null;
}