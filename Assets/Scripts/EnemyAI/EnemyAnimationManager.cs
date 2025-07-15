using UnityEngine;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using DG.Tweening;
using System;
using System.Collections;
using static UnityEngine.EventSystems.EventTrigger;


namespace EnemyAI {
    [RequireComponent(typeof(SkeletonAnimation))]
    public class EnemyAnimationManager : MonoBehaviour {

        public bool IsDashing = false;
        private Coroutine _waitForAnimationCoroutine;

        [Header("Spine Animation")]
        [Tooltip("Reference to the SkeletonAnimation component")]
        public SkeletonAnimation skeletonAnimation;

        [Header("Animation Names")]
        [SerializeField] private List<string> patrolIdleAnimations = new List<string>();
        [SerializeField] private List<string> permanentIdleAnimations = new List<string>();
        [SerializeField] private string alertSearchAnimName;
        [SerializeField] private string catchAnimName;
        [SerializeField] private string runAnimName;
        [SerializeField] private string run2AnimName;
        [SerializeField] private string walkAnimName;
        [SerializeField] private string walkSearchAnimName;
        [SerializeField] private string stopInPlaceSearchAnimName;

        [Header("Animation Settings")]
        [Tooltip("Time in seconds the enemy will stay on the ground after dashing towards the player")]
        [SerializeField] private float onGroundDuration = 2.0f;

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

        private string GetRandomAnimation(List<string> animList)
        {
            return animList[UnityEngine.Random.Range(0, animList.Count)];
        }

        /// <summary>
        /// Convenience for choosing animation by state name.
        /// </summary>
        public void SetCharacterState(EnemyStateType state, bool isStop = false) {
            // Reset dashing state, since if the character state changes forcibly, we assume the dash is done.
            IsDashing = false;
            if (null != _waitForAnimationCoroutine)
            {
                StopCoroutine(_waitForAnimationCoroutine);
            }

            switch (state) {
                case EnemyStateType.PatrolIdle:
                    spineState.SetAnimation(0, GetRandomAnimation(patrolIdleAnimations), true);
                    break;
                case EnemyStateType.PermanentIdle:
                    spineState.SetAnimation(0, GetRandomAnimation(permanentIdleAnimations), true);
                    break;
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

            IsDashing = true;

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
            spineState.AddAnimation(0, alertSearchAnimName, false, onGroundDuration);
            TrackEntry track = spineState.AddAnimation(0, permanentIdleAnimations[0],  false, 0);
            track.TimeScale = 3.0f; // Speeding up the idle animation as its main purpose is to get the enemy back on its feet
            _waitForAnimationCoroutine = StartCoroutine(WaitForAnimation(track));
        }

        private IEnumerator WaitForAnimation(TrackEntry track)
        {
            yield return new WaitForSpineAnimationComplete(track);
            IsDashing = false;
        }
    }
}
