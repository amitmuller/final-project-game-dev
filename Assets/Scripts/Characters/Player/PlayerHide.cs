using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Interactable_objects;

namespace Characters.Player
{
    public class PlayerHide : MonoBehaviour
    {
        [SerializeField] private GameObject bodyVisual; // Assign the visible sprite or mesh here
        private MonoBehaviour movementScript; // Drag your movement controller script here

        private HidableObject nearbyHidable;
        private bool isHiding = false;
        private Color originalColor;


        private void Start()
        {
            movementScript = GetComponent<characterMovement>();
            originalColor = bodyVisual.GetComponent<SpriteRenderer>().color;
        }

        public void SetNearbyHidable(HidableObject hidable)
        {
            nearbyHidable = hidable;
        }

        public bool IsHiding() => isHiding;

        public void OnHide(InputAction.CallbackContext context)
        {
            if (nearbyHidable != null)
            {
                if (!isHiding)
                    EnterHide();
                else
                    ExitHide();
            }
        }
        

        private void EnterHide()
        {
            Color overlayColor = new Color(0.4121f, 0.4346f, 0.4622f, 1f);
            bodyVisual.GetComponent<SpriteRenderer>().color = overlayColor;
            // Or play hiding animation
            movementScript.enabled = false;
            isHiding = true;
        }

        private void ExitHide()
        {
            bodyVisual.GetComponent<SpriteRenderer>().color = originalColor;
            movementScript.enabled = true;
            isHiding = false;
        }
    }
}