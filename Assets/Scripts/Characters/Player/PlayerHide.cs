using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Interactable_objects;
using Interactable_objects.object_utills.enums;

namespace Characters.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerHide : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private GameObject bodyVisual;

        [SerializeField] private MonoBehaviour movementScript;
        [SerializeField] private SpriteRenderer bodyRenderer;

        [Header("Sorting Layers")] [SerializeField]
        private string layerHiddenBack = "HiddenBackPlayer";

        [SerializeField] private string layerDefault = "Player";
        [SerializeField] private string layerHiddenFront = "HiddenFrontPlayer";

        [Header("Hide Lanes (Y positions)")]
        [Tooltip("Y when hiding behind back furniture (higher line)")]
        [Range(-10, 10)]
        [SerializeField]
        private float hideYBack;

        [Tooltip("Y when hiding in front of front furniture (lower line)")] [Range(-10, 10)] [SerializeField]
        private float hideYFront;

        [Header("Edge-exit tolerance")] [SerializeField, Min(0.01f)]
        private float edgeTolerance = 1f;

        [SerializeField] private float hideEdgeOffset = 1f;

        private HidableObject currentHidable;
        private bool isHiding;
        private Color originalColor;
        private float targetHideY;
        private float originalY;
        private Collider2D playerCollider;


        private void Awake()
        {
            if (!bodyRenderer) bodyRenderer = bodyVisual.GetComponent<SpriteRenderer>();
            originalColor = bodyRenderer.color;
            originalY = transform.position.y;
            playerCollider = GetComponent<Collider2D>();

        }

        public void SetNearbyHidable(HidableObject hidable) => currentHidable = hidable;
        public bool IsHiding() => isHiding;

        public void OnHide(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed || currentHidable == null) return;
            if (!isHiding && AtEdge())
                EnterHide();
            else if (AtEdge())
                ExitHide();
            // else
            //     ExitHide();
        }

        private void EnterHide()
        {
            // tint & sorting layer
            bodyRenderer.color = new Color(0.36f, 0.4f, 0.43f, 1f);
            bodyRenderer.sortingLayerName = currentHidable.Layer == HideLayer.Back
                ? layerHiddenBack
                : layerHiddenFront;

            // choose the Y-lane
            targetHideY = currentHidable.Layer == HideLayer.Back
                ? hideYBack
                : hideYFront;

            isHiding = true;
        }

        private void ExitHide()
        {
            // restore visuals
            bodyRenderer.color = originalColor;
            bodyRenderer.sortingLayerName = layerDefault;
            // restore position
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, originalY, pos.z);
            isHiding = false;
        }

        private void Update()
        {
            if(!isHiding && currentHidable != null && AtEdge())
            {
                currentHidable.GetComponent<HidableObject>().setIndicator(true);
                return;
            }

            if (!isHiding && currentHidable != null && !AtEdge())
            {
                currentHidable.GetComponent<HidableObject>().setIndicator(false);
            }
            if (!isHiding || currentHidable == null)
            {
                return;
            }
            
            currentHidable.GetComponent<HidableObject>().setIndicator(false);


            float x = Mathf.Clamp(transform.position.x,
                currentHidable.LeftX,
                currentHidable.RightX);


            transform.position = new Vector3(x, targetHideY, transform.position.z);
        }

        private bool AtEdge()
        {
            Bounds bounds = playerCollider.bounds;
            float leftEdge = bounds.min.x;
            float rightEdge = bounds.max.x;
            return Mathf.Abs(rightEdge - currentHidable.LeftX) <= edgeTolerance ||
                   Mathf.Abs(leftEdge - currentHidable.RightX) <= edgeTolerance || 
                   Mathf.Abs(transform.position.x - currentHidable.LeftX) <= edgeTolerance || 
                   Mathf.Abs(transform.position.x  - currentHidable.RightX) <= edgeTolerance;
        }

        private void OnDrawGizmos()
        {
            if (!playerCollider)
                playerCollider = GetComponent<Collider2D>();

            Bounds bounds = playerCollider.bounds;
            float y = transform.position.y;

            // Draw player collider edges (left and right)
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(bounds.min.x, y - 0.25f, 0), new Vector3(bounds.min.x, y + 0.25f, 0));
            Gizmos.DrawLine(new Vector3(bounds.max.x, y - 0.25f, 0), new Vector3(bounds.max.x, y + 0.25f, 0));

            // Draw line connecting the edges (optional)
            Gizmos.DrawLine(new Vector3(bounds.min.x, y, 0), new Vector3(bounds.max.x, y, 0));

            // Draw edge tolerance zones near HidableObject edges
            if (currentHidable != null)
            {
                Gizmos.color = Color.yellow;

                // Left edge + tolerance
                Gizmos.DrawWireCube(new Vector3(currentHidable.LeftX, y, 0),
                    new Vector3(edgeTolerance * 2, 0.5f, 0));

                // Right edge + tolerance
                Gizmos.DrawWireCube(new Vector3(currentHidable.RightX, y, 0),
                    new Vector3(edgeTolerance * 2, 0.5f, 0));

                // Draw exact left/right edges of the hidable
                Gizmos.color = Color.green;
                Gizmos.DrawLine(new Vector3(currentHidable.LeftX, y - 0.5f, 0),
                    new Vector3(currentHidable.LeftX, y + 0.5f, 0));
            }

        }
    }
}