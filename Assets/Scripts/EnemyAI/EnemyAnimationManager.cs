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
        

        private Spine.AnimationState spineState;

        void Awake() {
            if (skeletonAnimation == null)
                skeletonAnimation = GetComponent<SkeletonAnimation>();
            spineState = skeletonAnimation.AnimationState;
        }

        /// <summary>Convenience for choosing animation by state name.</summary>
        public void SetCharacterState(EnemyStateType state) {
            switch (state) {
                case EnemyStateType.Calm:     spineState.SetAnimation(0, walkAnimName, true);       break;
                case EnemyStateType.Alert:    spineState.SetAnimation(0,runAnimName, false);     break;
                case EnemyStateType.Searching: spineState.SetAnimation(0,alertSearchAnimName, true);     break;
                case EnemyStateType.Chase:    spineState.SetAnimation(0,runAnimName, true);        break;
                // add more cases or mapping as needed
                default:
                    Debug.LogWarning($"[Anim] Unknown state '{state.ToString()}'");
                    break;
            }
        }
    }
}

