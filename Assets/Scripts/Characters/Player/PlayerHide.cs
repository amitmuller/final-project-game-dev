using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Interactable_objects;
using Interactable_objects.object_utills.enums;
using Spine.Unity;

namespace Characters.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerHide : MonoBehaviour
    {
        [Header("References")]
        private characterMovement playerMove;
        [SerializeField] private GameObject bodyVisual;
        [SerializeField] private MonoBehaviour movementScript;
        [SerializeField] private SpriteRenderer bodyRenderer;

        [Header("Hide Orders (within Default layer)")]
        [Tooltip("Player’s normal drawing order")]
        [SerializeField] private int normalOrder      = 11;
        [Tooltip("Order to use when hiding behind back furniture (back furniture = 10)")]
        [SerializeField] private int hiddenBackOrder  = 1;
        [Tooltip("Order to use when hiding in front of front furniture (front furniture = 20)")]
        [SerializeField] private int hiddenFrontOrder = 21;

        [Header("Hide Lanes (Y positions)")]
        [Tooltip("Y when hiding behind back furniture (higher line)")]
        [Range(-10, 10)]
        [SerializeField] private float hideYBack;
        [Tooltip("Y when hiding in front of front furniture (lower line)")]
        [Range(-10, 10)]
        [SerializeField] private float hideYFront;

        [Header("Edge-exit tolerance")]
        [SerializeField, Min(0.01f)] private float edgeTolerance = 1f;
        [SerializeField] private float hideEdgeOffset = 1f;
        
        [SerializeField] private SkeletonAnimation rendereSkeletonAnimation;
        [SerializeField] private MeshRenderer meshRenderer;
        private const float  PeekThreshold = 0.95f;

        private HidableObject currentHidable;
        private bool          isHiding;
        private Color         originalColor;
        private int           originalOrder;
        private float         targetHideY;
        private float         originalY;
        private Collider2D    playerCollider;
        private Transform blurTf;

        private void Awake()
        {
            playerMove = GetComponent<characterMovement>();
            // Grab the SpriteRenderer if not assigned
            //if (!bodyRenderer)
              //  bodyRenderer = bodyVisual.GetComponent<SpriteRenderer>();
            
            //rendereSkeletonAnimation = GetComponent<SkeletonAnimation>();
            meshRenderer = GetComponent<MeshRenderer>();
            //originalColor   = bodyRenderer.color;
            originalOrder   = meshRenderer.sortingOrder;
            originalY       = transform.position.y;
            playerCollider  = GetComponent<Collider2D>();
            blurTf = transform.Find("BlurScreen");
            // Ensure we start at our normal order
            meshRenderer.sortingOrder = normalOrder;
        }

        // Called by HidableObject when player comes near
        public void SetNearbyHidable(HidableObject hidable) => currentHidable = hidable;
        public bool IsHiding() => isHiding;
        
        // Bound to your InputAction for "Hide"
        public void OnHide(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed || currentHidable == null) return;

            if (!isHiding && AtEdge())
                EnterHide();
            else if (isHiding && AtEdge())
                ExitHide();
        }

        private void EnterHide()
        {
            // Darken sprite
            bodyRenderer.color = new Color(0.36f, 0.4f, 0.43f, 1f);

            // Behind or in front?
            if (currentHidable.Layer == HideLayer.Back)
            {
                meshRenderer.sortingOrder = hiddenBackOrder;
                targetHideY               = hideYBack;
            }
            else
            {
                //bodyRenderer.sortingOrder = hiddenFrontOrder;
                meshRenderer.sortingOrder = hiddenFrontOrder;
                targetHideY               = hideYFront;
            }
            if(blurTf != null)
                blurTf.gameObject.SetActive(true);

            isHiding = true;
        }

        private void ExitHide()
        {
            // Restore visuals
            //bodyRenderer.color        = originalColor;
            meshRenderer.sortingOrder = originalOrder;

            // Snap back to original Y
            var pos = transform.position;
            transform.position = new Vector3(pos.x, originalY, pos.z);
            if(blurTf != null)
                blurTf.gameObject.SetActive(false);
            isHiding = false;
        }

        private void Update()
        {
            Debug.Log("moveinput : "+ playerMove.MoveInput);
            // Show/hide indicator when near edge
            if (!isHiding && currentHidable != null)
            {
                currentHidable.setIndicator(AtEdge());
                return;
            }

            // While hiding, lock to hide-lane and within bounds
            if (isHiding && currentHidable != null)
            {
                currentHidable.setIndicator(false);

                float clampedX = Mathf.Clamp(transform.position.x,
                                             currentHidable.LeftX,
                                             currentHidable.RightX);

                transform.position = new Vector3(clampedX, targetHideY, transform.position.z);
            }

            if (isHiding)
            {
                PeekWhileHiding();
            }
        }


        private void PeekWhileHiding()
        {
            if (playerMove != null)
            {
                var move = playerMove.MoveInput;
                var peek   = move.y > PeekThreshold;
                if (peek) playerMove.SetCanMove(false); else playerMove.SetCanMove(true);
                if (blurTf != null)
                    blurTf.gameObject.SetActive(!peek);
            }
        }

        // True if player collider is within edgeTolerance of hidable’s edges
        private bool AtEdge()
        {
            var bounds    = playerCollider.bounds;
            var leftEdge  = bounds.min.x;
            var rightEdge = bounds.max.x;
            var px        = transform.position.x;

            return
                Mathf.Abs(rightEdge - currentHidable.LeftX)  <= edgeTolerance ||
                Mathf.Abs(leftEdge  - currentHidable.RightX) <= edgeTolerance ||
                Mathf.Abs(px        - currentHidable.LeftX)  <= edgeTolerance ||
                Mathf.Abs(px        - currentHidable.RightX) <= edgeTolerance;
        }

        private void OnDrawGizmos()
        {
            if (!playerCollider)
                playerCollider = GetComponent<Collider2D>();

            var bounds = playerCollider.bounds;
            var y      = transform.position.y;

            // Player collider edges
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(bounds.min.x, y - 0.25f, 0),
                            new Vector3(bounds.min.x, y + 0.25f, 0));
            Gizmos.DrawLine(new Vector3(bounds.max.x, y - 0.25f, 0),
                            new Vector3(bounds.max.x, y + 0.25f, 0));
            Gizmos.DrawLine(new Vector3(bounds.min.x, y, 0),
                            new Vector3(bounds.max.x, y, 0));

            if (currentHidable != null)
            {
                // Tolerance zones
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(new Vector3(currentHidable.LeftX,  y, 0),
                                    new Vector3(edgeTolerance * 2, 0.5f, 0));
                Gizmos.DrawWireCube(new Vector3(currentHidable.RightX, y, 0),
                                    new Vector3(edgeTolerance * 2, 0.5f, 0));

                // Exact hidable edges
                Gizmos.color = Color.green;
                Gizmos.DrawLine(new Vector3(currentHidable.LeftX,  y - 0.5f, 0),
                                new Vector3(currentHidable.LeftX,  y + 0.5f, 0));
                Gizmos.DrawLine(new Vector3(currentHidable.RightX, y - 0.5f, 0),
                                new Vector3(currentHidable.RightX, y + 0.5f, 0));
            }
        }
    }
}
