// EnemyAnimationManager.cs
using UnityEngine;
using Spine;
using Spine.Unity;
using DG.Tweening;


namespace EnemyAI {
    [RequireComponent(typeof(SkeletonAnimation))]
    public class EnemyAnimationManager : MonoBehaviour {
        [Header("Spine Animation")]
        [Tooltip("Reference to the SkeletonAnimation component")]
        public SkeletonAnimation skeletonAnimation;

        [Header("Animation Names")]
        [SerializeField] private string idleAnimName;
        [SerializeField] private string alertSearchAnimName;
        [SerializeField] private string catchAnimName;
        [SerializeField] private string idleHandoutAnimName;
        [SerializeField] private string runAnimName;
        [SerializeField] private string run2AnimName;
        [SerializeField] private string walkAnimName;
        [SerializeField] private string walkSearchAnimName;
        [SerializeField] private string stopInPlaceSearchAnimName;

        [Header("Collider")]
        [Tooltip("Body collider for all animations")]
        [SerializeField] private Collider2D bodyCollider;
        private Rigidbody2D _rigidbody2D;
        private Spine.AnimationState spineState;
        private Vector3 originalColliderEuler;

        void Awake() {
            _rigidbody2D    = GetComponent<Rigidbody2D>();
            if (skeletonAnimation == null)
                skeletonAnimation = GetComponent<SkeletonAnimation>();
            spineState = skeletonAnimation.AnimationState;
            originalColliderEuler = bodyCollider.transform.localEulerAngles;
        }

        /// <summary>
        /// Convenience for choosing animation by state name.
        /// </summary>
        public void SetCharacterState(EnemyStateType state, bool isStop = false) {
            switch (state) {
                case EnemyStateType.Calm:
                    spineState.SetAnimation(0, walkAnimName, true);
                    break;
                case EnemyStateType.Alert:
                    spineState.SetAnimation(0, walkSearchAnimName, true);
                    break;
                case EnemyStateType.Searching:
                    if (isStop)
                        spineState.SetAnimation(0, stopInPlaceSearchAnimName, true);
                    else
                        spineState.SetAnimation(0, walkSearchAnimName, true);
                    break;
                case EnemyStateType.Chase:
                    spineState.SetAnimation(0, alertSearchAnimName, false);
                    var chaseEntry = spineState.AddAnimation(0, run2AnimName, true, 0);
                    chaseEntry.TimeScale = 0.95f;
                    break;
                default:
                    Debug.LogWarning($"[Anim] Unknown state '{state}'");
                    break;
            }
        }

        /// <summary>
        /// Play dash once, rotate collider during dash, then queue follow-up animations.
        /// </summary>
        /// <param name="dashDuration">How long the dash lasts (seconds)</param>
        public void PlayDash(float dashDuration = 0.5f) {
            // Play the dash (catch) animation
            var dashEntry = spineState.SetAnimation(0, catchAnimName, false);
            dashEntry.TimeScale = 1.2f;
            // Determine rotation based on facing direction (scale.x)
            float angle = _rigidbody2D.linearVelocity.x < 0.01f  ?  90f : -90f;

            // Create a DOTween sequence
            var seq = DOTween.Sequence();
            // Rotate collider into dash position
            seq.Append(bodyCollider.transform.DOLocalRotate(new Vector3(0, 0, angle), 0.8f));

            

            // Keep rotated during dash
            seq.AppendInterval(dashDuration);

            // Rotate collider back to its original orientation
            seq.Append(bodyCollider.transform.DOLocalRotate(originalColliderEuler, 0.1f));

            // Queue follow-up animations
            spineState.AddAnimation(0, alertSearchAnimName, false, 0);
            spineState.AddAnimation(0, idleHandoutAnimName,  false, 0);
            spineState.AddAnimation(0, run2AnimName,          true, 0);
        }
    }
}
