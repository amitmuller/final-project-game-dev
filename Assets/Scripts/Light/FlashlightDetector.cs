
using UnityEngine;
using Characters.Player;

public class FlashlightDetector : MonoBehaviour
{
    [Tooltip("Reference to the parent guard logic script")]
    public SecurityFlashlight guard;

    private void OnTriggerStay2D(Collider2D other)
    {
        var hide = other.GetComponent<PlayerHide>();
        if (hide && !hide.IsHiding())
        {
            guard.PlayerSpotted(other.gameObject);   // delegate to the main script
        }
    }
}

