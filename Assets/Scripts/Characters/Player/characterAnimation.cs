// Characters/Player/characterAnimation.cs

using System.Collections;
using UnityEngine;
using Spine.Unity;
using Spine;

namespace Characters.Player
{
    public enum PlayerAnimState
    {
        Idle,
        Walk,
        HideEnterUp,     // back-furniture hide
        HideEnterDown,   // front-furniture hide
        HideIdle,        // waiting in hide
        HideWalk,        // (if you ever allow walking while hidden)
        Peek,
        TailAim,
        TailPick,
        TailThrow
    }

    [RequireComponent(typeof(SkeletonAnimation))]
    public class characterAnimation : MonoBehaviour
    {
        [Header("Spine Clips")]
        public SkeletonAnimation skeletonAnimation;
        
        
        
        [Header("Animation Names")]
        public string idleName          = "idle";
        public string walkName          = "walking";
        public string hideUpName        = "intoHiding";
        public string hideDownName      = "intoHidingDown";
        public string hideIdleName      = "walkingHiding";
        public string peekName          = "PeekingHeadUp";
        public string tailAimName       = "TailAim";
        public string tailPickName      = "tailPick";
        public string tailThrowName     = "tailThrow";
        public string blinkName         = "blink";

        PlayerAnimState _state = PlayerAnimState.Idle;
        [Tooltip("Min/max seconds between blinks")]
        public Vector2 blinkInterval = new Vector2(3f, 8f);

        void Awake()
        {
            if (skeletonAnimation == null)
                skeletonAnimation = GetComponent<SkeletonAnimation>();
            TransitionTo(PlayerAnimState.Idle);
            StartCoroutine(BlinkLoop());
        }
        IEnumerator BlinkLoop()
        {
            if (skeletonAnimation == null)
            {
                Debug.LogWarning("BlinkLoop will not run because skeletonAnimation or blink is null.");
                yield break;
            }

            while (true)
            {
                // wait a random time
                float wait = Random.Range(blinkInterval.x, blinkInterval.y);
                yield return new WaitForSeconds(wait);

                // play the blink on track 2 (higher than your other tracks)
                print("blink");
                var entry = skeletonAnimation.state.SetAnimation(2, blinkName, false);

                entry.Complete += e =>
                {
                    skeletonAnimation.state.SetEmptyAnimation(1, 0.1f);
                };
            }
        }

        public void TransitionTo(PlayerAnimState newState)
        {
            if (_state == newState) return;
            _state = newState;

            var state = skeletonAnimation.state;
            TrackEntry entry = null;
            print(newState);
            switch (newState)
            {
                // ─── Track 0 (base) ────────────────────────────────────────
                case PlayerAnimState.Idle:
                    entry = state.SetAnimation(0, idleName, true);
                    break;
                case PlayerAnimState.Walk:
                    entry = state.SetAnimation(0, walkName, true);
                    break;
                case PlayerAnimState.HideEnterUp:
                    entry = state.SetAnimation(0, hideUpName, false);
                    entry.Complete += e => TransitionTo(PlayerAnimState.Idle);
                    break;
                case PlayerAnimState.HideEnterDown:
                    entry = state.SetAnimation(0, hideDownName, false);
                    entry.Complete += e => TransitionTo(PlayerAnimState.Idle);
                    break;
                case PlayerAnimState.HideIdle:
                case PlayerAnimState.HideWalk:
                    entry = state.SetAnimation(0, hideIdleName, true);
                    break;
                case PlayerAnimState.Peek:
                    entry = state.SetAnimation(0, peekName, true);
                    break;

                // ─── Track 1 (tail overlay) ─────────────────────────────────
                case PlayerAnimState.TailAim:
                    entry = state.SetAnimation(1, tailAimName, true);
                    break;
                case PlayerAnimState.TailPick:
                    entry = state.SetAnimation(1, tailPickName, false);
                    entry.Complete += e => state.SetEmptyAnimation(1, 0.1f);
                    break;
                case PlayerAnimState.TailThrow:
                    entry = state.SetAnimation(1, tailThrowName, false);
                    entry.Complete += e =>
                    {
                        state.SetEmptyAnimation(1, 0.1f);
                        TransitionTo(PlayerAnimState.Idle);
                    };
                    break;
                
            }

            // ensure a non-zero time scale
            if (entry != null && entry.TimeScale == 0f)
                entry.TimeScale = 1f;
        }
    }
}
