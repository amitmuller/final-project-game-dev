using UnityEngine;
using Interactable_objects.object_utills.enums;

namespace Interactable_objects
{
    [RequireComponent(typeof(Collider2D))]
    public class HidableObject : MonoBehaviour
    {
        [Header("Hide boundaries")]
        [SerializeField] private Transform leftEdge;
        [SerializeField] private Transform rightEdge;

        [Header("Rendering")]
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