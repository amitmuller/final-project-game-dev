using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class TailPushController : MonoBehaviour
{

    private readonly List<PushableObject> overlappedPushables = new List<PushableObject>();
    
    public void OnPush(InputAction.CallbackContext ctx)
    {
        Debug.Log("OnPush");
        if (ctx.performed)
            TriggerAllPushables();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PushableObject>(out var pushable))
        {
            if (!overlappedPushables.Contains(pushable))
                overlappedPushables.Add(pushable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PushableObject>(out var pushable))
            overlappedPushables.Remove(pushable);
    }

    private void TriggerAllPushables()
    {
        Debug.Log($"TriggerAllPushables: {overlappedPushables.Count}");
        foreach (var pushable in overlappedPushables)
            pushable.TriggerFall();
    }
}