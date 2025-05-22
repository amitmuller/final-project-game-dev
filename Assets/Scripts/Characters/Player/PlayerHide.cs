using UnityEngine;
using UnityEngine.InputSystem;
using Interactable_objects;
using Interactable_objects.object_utills.enums;

namespace Characters.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerHide : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject bodyVisual;
        [SerializeField] private MonoBehaviour movementScript;
        [SerializeField] private SpriteRenderer bodyRenderer;

        [Header("Sorting Layers")]
        [SerializeField] private string layerHiddenBack  = "HiddenBackPlayer";
        [SerializeField] private string layerDefault     = "Player";
        [SerializeField] private string layerHiddenFront = "HiddenFrontPlayer";

        [Header("Hide Lanes (Y positions)")]
        [Tooltip("Y when hiding behind back furniture (higher line)")]
        [Range(-10, 10)] [SerializeField] private float hideYBack;
        [Tooltip("Y when hiding in front of front furniture (lower line)")]
        [Range(-10, 10)][SerializeField] private float hideYFront;

        [Header("Edge-exit tolerance")]
        [SerializeField, Min(0.01f)] private float edgeTolerance = 0.05f;

        private HidableObject currentHidable;
        private bool isHiding;
        private Color originalColor;
        private float targetHideY;
        private float originalY;
        private void Awake()
        {
            if (!bodyRenderer) bodyRenderer = bodyVisual.GetComponent<SpriteRenderer>();
            originalColor = bodyRenderer.color;
            originalY = transform.position.y;
        }

        public void SetNearbyHidable(HidableObject hidable) => currentHidable = hidable;
        public bool IsHiding()                         => isHiding;

        public void OnHide(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed || currentHidable == null) return;
            Debug.Log(AtEdge());
            if (!isHiding)
                EnterHide();
            else if (AtEdge())
                ExitHide();
        }

        private void EnterHide()
        {
            // tint & sorting layer
            bodyRenderer.color            = new Color(0.36f,0.4f,0.43f,1f);
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
            Debug.Log("Exiting Hide");
            // restore visuals
            bodyRenderer.color            = originalColor;
            bodyRenderer.sortingLayerName = layerDefault;
            // restore position
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, originalY, pos.z);
            isHiding = false;
        }

        private void Update()
        {
            if (!isHiding || currentHidable == null) return;

            
            float x = Mathf.Clamp(transform.position.x,
                                  currentHidable.LeftX,
                                  currentHidable.RightX);

            
            transform.position = new Vector3(x, targetHideY, transform.position.z);
        }

        private bool AtEdge()
        {
            float x = transform.position.x;
            return Mathf.Abs(x - currentHidable.LeftX)  <= edgeTolerance ||
                   Mathf.Abs(x - currentHidable.RightX) <= edgeTolerance;
        }
    }
}
