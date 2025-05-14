using UnityEngine;
using Interactable_objects.object_utills.enums;

namespace Interactable_objects
{
    [RequireComponent(typeof(Collider2D))]
    public class HidableObject : MonoBehaviour
    {
        [Header("Hide boundaries (world-space)")]
        [Tooltip("Empty at the LEFT edge of the sprite")]
        [SerializeField] private Transform leftEdge;
        [Tooltip("Empty at the RIGHT edge of the sprite")]
        [SerializeField] private Transform rightEdge;

        [Header("Rendering")]
        [Tooltip("Does the player hide BEHIND (Back) or IN FRONT (Front) of this furniture?")]
        [SerializeField] private HideLayer hideLayer = HideLayer.Back;

        /*  Properties for PlayerHide  */
        public float LeftX  => leftEdge.position.x;
        public float RightX => rightEdge.position.x;
        public HideLayer Layer => hideLayer;

        /*  Let PlayerHide know we’re nearby  */
        private void OnTriggerEnter2D(Collider2D other)
        {
            var ph = other.GetComponent<Characters.Player.PlayerHide>();
            if (ph) ph.SetNearbyHidable(this);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var ph = other.GetComponent<Characters.Player.PlayerHide>();
            if (ph && !ph.IsHiding()) ph.SetNearbyHidable(null);
        }
    }
}