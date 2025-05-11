using UnityEngine;

namespace Interactable_objects
{
    public class HidableObject : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var playerHide = other.GetComponent<Characters.Player.PlayerHide>();
            if (playerHide != null)
            {
                playerHide.SetNearbyHidable(this);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var playerHide = other.GetComponent<Characters.Player.PlayerHide>();
            if (playerHide != null && !playerHide.IsHiding())
            {
                playerHide.SetNearbyHidable(null);
            }
        }
    }
}