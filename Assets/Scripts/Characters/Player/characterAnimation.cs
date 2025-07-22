// Characters/Player/characterAnimation.cs

using System;
using System.Collections;
using UnityEngine;
using Spine;
using Spine.Unity;
using System.Collections;

namespace Characters.Player
{
    public enum PlayerAnimState
    {
        Idle,
        Walk,
        HideEnterUp,
        HideEnterDown,
        HideIdle,
        HideWalk,
        Peek,
        TailAim,
        TailPick,
        TailThrow,
        Caught,
        Sleep,
        SleepyEyes,
        Awake,
    }

    [RequireComponent(typeof(SkeletonAnimation))]
    public class characterAnimation : MonoBehaviour
    {
        [Header("Spine Clips")] public SkeletonAnimation skeletonAnimation;

        [Header("Animation Names")] 
        public string idleName = "idle";
        public string walkName = "walking";
        public string hideUpName = "intoHiding";
        public string hideDownName = "intoHidingDown";
        public string hideIdleName = "idleHiding";
        public string hideWalkName = "walkingHiding";
        public string peekName = "PeekingHeadUp";
        public string tailAimName = "TailAim";
        public string tailPickName = "tailPick";
        public string tailThrowName = "tailThrow";
        public string blinkName = "blink";
        public string caughtName = "caught";
        public string sleepName = "sleeping";
        public string awakeName = "awake";
        public string SleepyEyesName = "SleepyEyes";
        

        private PlayerAnimState _state = PlayerAnimState.Idle;
        [Tooltip("Min/max seconds between blinks")] public Vector2 blinkInterval = new Vector2(3f, 8f);

        void Awake()
        {
            if (skeletonAnimation == null)
                skeletonAnimation = GetComponent<SkeletonAnimation>();
            // start idle and blinking
            // TransitionTo(PlayerAnimState.Idle);
        }

        public void startBlink()
        {
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
                float wait = UnityEngine. Random.Range(blinkInterval.x, blinkInterval.y);
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

        /// <summary>
        /// Simple transition without callback.
        /// </summary>
        public void TransitionTo(PlayerAnimState newState)
        {
            TransitionTo(newState, null);
        }

        /// <summary>
        /// Transition to a new state and optionally run a callback when the clip completes.
        /// </summary>
        public TrackEntry TransitionTo(PlayerAnimState newState, Spine.AnimationState.TrackEntryDelegate onComplete)
        {
            // avoid re-transition
            if (_state == newState) return null;
            _state = newState;

            var state = skeletonAnimation.state;
            TrackEntry entry = null;
            
            print(newState.ToString());

            // clear overlays if caught
            if (newState == PlayerAnimState.Caught || newState == PlayerAnimState.Sleep || 
                newState == PlayerAnimState.SleepyEyes)
            {
                state.ClearTrack(1);
                state.ClearTrack(2);
            }

            switch (newState)
            {
                case PlayerAnimState.Idle:
                    entry = state.SetAnimation(0, idleName, true);
                    break;
                case PlayerAnimState.Walk:
                    entry = state.SetAnimation(0, walkName, true);
                    break;
                case PlayerAnimState.HideEnterUp:
                    entry = state.SetAnimation(0, hideUpName, false);
                    entry.Complete += e => TransitionTo(PlayerAnimState.HideIdle);
                    break;
                case PlayerAnimState.HideEnterDown:
                    entry = state.SetAnimation(0, hideDownName, false);
                    entry.Complete += e => TransitionTo(PlayerAnimState.HideIdle);
                    break;
                case PlayerAnimState.HideIdle:
                    entry = state.SetAnimation(0, hideIdleName, true);
                    break;
                case PlayerAnimState.HideWalk:
                    entry = state.SetAnimation(0, hideWalkName, true);
                    break;
                case PlayerAnimState.Peek:
                    entry = state.SetAnimation(0, peekName, true);
                    break;
                case PlayerAnimState.Caught:
                    entry = state.SetAnimation(0, caughtName, false);
                    break;
                case PlayerAnimState.Sleep:
                    entry = state.SetAnimation(0, sleepName, true);
                    break;
                case PlayerAnimState.Awake:
                    entry = state.SetAnimation(0, awakeName, false);
                    entry.Complete += e => TransitionTo(PlayerAnimState.Idle);
                    break;
                case PlayerAnimState.SleepyEyes:
                    entry = state.SetAnimation(0, SleepyEyesName, false);
                    entry.Complete += e => TransitionTo(PlayerAnimState.Sleep);
                    break;
                case PlayerAnimState.TailAim:
                    entry = state.SetAnimation(1, tailAimName, true);
                    entry.Complete += e => state.SetEmptyAnimation(1, 0.1f);
                    break;
                case PlayerAnimState.TailPick:
                    entry = state.SetAnimation(1, tailPickName, false);
                    entry.Complete += e => state.SetEmptyAnimation(1, 0.1f);
                    break;
                case PlayerAnimState.TailThrow:
                    entry = state.SetAnimation(1, tailThrowName, false);
                    entry.Complete += e => state.SetEmptyAnimation(1, 0.1f);
                    break;
            }

            if (entry != null)
            {
                if (entry.TimeScale == 0f)
                    entry.TimeScale = 1f;

                // external callback
                if (onComplete != null)
                    entry.Complete += onComplete;
            }

            return entry;
        }
    }
}
