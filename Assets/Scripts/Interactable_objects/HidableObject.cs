using System;
using UnityEngine;
using Interactable_objects.object_utills.enums;

namespace Interactable_objects
{
    [RequireComponent(typeof(Collider2D))]
    public class HidableObject : MonoBehaviour
    {

        [Header("Rendering")]
        [SerializeField] private HideLayer hideLayer = HideLayer.Back;
        public HideLayer Layer => hideLayer;

        [Header("Hide boundaries")]
        public float LeftX;
        public float RightX;

        /*  Properties for PlayerHide  */

        private void Start()
        {
            var bounds = GetComponent<Collider2D>().bounds;
            LeftX = bounds.min.x;
            RightX = bounds.max.x;
        }

        
        

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