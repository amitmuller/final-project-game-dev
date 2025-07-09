// EnemyAnimationManager.cs
using UnityEngine;
using Spine;
using Spine.Unity;

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
        
        [Header("Colliders")]
        [Tooltip("Default body collider for walk/run animations")]
        [SerializeField] private Collider2D bodyCollider;
        [Tooltip("Alternate collider for catch/dash animations")]
        [SerializeField] private Collider2D attackCollider;

        private Spine.AnimationState spineState;

        void Awake() {
            if (skeletonAnimation == null)
                skeletonAnimation = GetComponent<SkeletonAnimation>();
            spineState = skeletonAnimation.AnimationState;
            spineState.Start += HandleAnimStart;
        }
        
        void OnDestroy() {
            if (spineState != null)
                spineState.Start -= HandleAnimStart;
        }
        
        private void HandleAnimStart(TrackEntry entry) {
            var animName = entry.Animation.Name;
            var isAttack = animName == catchAnimName;
            bodyCollider.enabled   = !isAttack;
            attackCollider.enabled = isAttack;
        }

        /// <summary>Convenience for choosing animation by state name.</summary>
        public void SetCharacterState(EnemyStateType state, bool isStop= false) {
            switch (state) {
                case EnemyStateType.Calm:
                    spineState.SetAnimation(0, walkAnimName, true);
                    break;
                case EnemyStateType.Alert:    
                    spineState.SetAnimation(0,walkSearchAnimName, true);     
                    break;
                case EnemyStateType.Searching:
                    if (isStop)
                    {
                        spineState.SetAnimation(0, stopInPlaceSearchAnimName, true);
                        
                    }
                    else
                    {
                        spineState.SetAnimation(0,walkSearchAnimName, true);
                    }
                    break;
                case EnemyStateType.Chase:    
                    spineState.SetAnimation(0, alertSearchAnimName, false);
                    spineState.AddAnimation(0, run2AnimName, true, 0);        
                    break;
                // add more cases or mapping as needed
                default:
                    Debug.LogWarning($"[Anim] Unknown state '{state.ToString()}'");
                    break;
            }
        }
        
            /// <summary>
            /// Play dash once, then queue chase‐run in loop.
            /// </summary>
            public void PlayDash() {
                    var catchEntry = spineState.SetAnimation(0, catchAnimName, false);
                    catchEntry.TimeScale = 1.2f;
                    spineState.AddAnimation(0,alertSearchAnimName, false, 0);
                    spineState.AddAnimation(0, run2AnimName, true, 0);
            }
    }
}

