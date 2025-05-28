using System.Collections;
using System.Linq;
using UnityEngine;
using EnemyAI;
using UnityEngine.Rendering.Universal;

namespace EnemyUtils
{
    public static class EnemyUtils
    {
        // ------------------------------- CALM STATE UTILS ------------------------------- //
        /// <summary>
        /// changes the state into chase mode if needed
        /// </summary>
        /// <param name="enemy"></param>
        public static void EnemyEnterChaseModeIfNeeded(EnemyAIController enemy)
        {
            var playerHidden  = enemy.IsPlayerHiding();
            var distToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);
            if (!playerHidden && distToPlayer <= enemy.detectionRange)
            {
                enemy.ChangeState(enemy.chaseState);
            }
        }

        /// <summary>
        /// Returns true if there’s at least one other enemy in the Calm state
        /// within `proximityRange` on X and both are on-screen.
        /// </summary>
        public static bool IsAnyCalmEnemyNearbyInCameraPosition(EnemyAIController self, float proximityRange)
        {
            return EnemyAIController.AllEnemies
                .Where(e => e != self && e.CurrentStateType == EnemyStateType.Calm)
                .Any(e =>
                    Mathf.Abs(self.transform.position.x - e.transform.position.x) <= proximityRange
                    && self.IsVisibleOnCamera()
                    && e.IsVisibleOnCamera()
                );
        }
        
        
        /// <summary>
        /// Returns true if we entered or are still in conversation,
        /// and invoked StopMovement() for this frame.
        /// otherwise returns false, and you should continue patrol.
        /// </summary>
        public static bool TryHandleConversation(EnemyAIController self, float proximityRange, 
                                                    float conversationDuration, float deltaTime)
        {
            // nothing to do?
            if (proximityRange <= 0f) return false;

            // any other calm enemy close & on‐screen?
            bool nearby = AllEnemiesNearby(self, proximityRange);
            if (nearby && !self.conversationCompleted)
            {
                // just started
                if (!self.isConversing)
                {
                    self.isConversing      = true;
                    self.conversationTimer = conversationDuration;
                }

                // stand still & tick
                self.conversationTimer -= deltaTime;
                self.StopMovement();

                // conversation done
                if (self.conversationTimer <= 0f)
                {
                    self.isConversing          = false;
                    self.conversationCompleted = true;
                }
                return true;
            }

            // if we moved apart, allow future chats
            if (!nearby)
                self.conversationCompleted = false;

            return false;
        }

        private static bool AllEnemiesNearby(EnemyAIController self, float range)
        {
            return EnemyAIController.AllEnemies
                .Where(e => e != self && e.CurrentStateType == EnemyStateType.Calm)
                .Any(e =>
                    Mathf.Abs(self.transform.position.x - e.transform.position.x) <= range
                    && self.IsVisibleOnCamera()
                    && e.IsVisibleOnCamera()
                );
        }
        
        /// <summary>
        /// Handles X-axis patrol and index advancement.
        /// </summary>
        public static void HandlePatrol(EnemyAIController self, float[] patrolPointsX, float patrolY, float speed, float threshold)
        {
            if (patrolPointsX != null && patrolPointsX.Length > 0)
            {
                float targetX = patrolPointsX[self.currentPatrolIndex];
                Vector2 targetPos = new Vector2(targetX, patrolY);
                self.MoveTowards(targetPos, speed);

                if (Mathf.Abs(self.transform.position.x - targetX) < threshold)
                {
                    self.currentPatrolIndex =
                        (self.currentPatrolIndex + 1) % patrolPointsX.Length;
                }
            }
            else
            {
                self.StopMovement();
            }
        }
        
        
        // ------------------------------- SEARCHING STATE UTILS ------------------------------- //
        
        /// <summary>
        /// When at noise position, enable flashlight and rotate the light left/right for the given duration.
        /// </summary>
        public static void SearchInNoisePosition(EnemyAIController enemy, float searchDuration)
        {
            var light2D = enemy.GetComponentInChildren<Light2D>();
            if (light2D == null)
                return;

            light2D.enabled = true;
            // start looking left/right on the light transform only
            enemy.StartCoroutine(LookAroundLightCoroutine(light2D.transform, searchDuration));
        }

        private static IEnumerator LookAroundLightCoroutine(Transform lightTransform, float duration)
        {
            float elapsed = 0f;
            var initialLocalRotation = lightTransform.localRotation;
            const float maxAngle = 45f;
            while (elapsed < duration)
            {
                // oscillate between -maxAngle and +maxAngle around Z
                float angle = Mathf.Sin(elapsed * Mathf.PI * 2f / duration) * maxAngle;
                lightTransform.localRotation = initialLocalRotation * Quaternion.Euler(0f, 0f, angle);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // reset
            lightTransform.localRotation = initialLocalRotation;
            var light2D = lightTransform.GetComponent<Light2D>();
            if (light2D != null)
                light2D.enabled = false;
        }
        
        // ------------------------------- ALERT STATE UTILS ------------------------------- //
        
        /// <summary>
        /// After alert timer expires, transition to Chase if the player is visible;
        /// otherwise set lastKnownNoisePosition and switch to Searching.
        /// </summary>
        public static void HandleAlertTransition(EnemyAIController enemy)
        {
            var playerHidden = enemy.IsPlayerHiding();
            var distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);
            var canSeePlayer = !playerHidden && distanceToPlayer <= enemy.detectionRange;

            if (canSeePlayer)
            {
                enemy.ChangeState(enemy.chaseState);
            }
            else
            {
                enemy.ChangeState(enemy.calmState);
            }
        }
        
        /// <summary>
        /// Patrol around last known noise position within a given proximity for a duration,
        /// enabling the flashlight during the patrol.
        /// </summary>
        public static void HandleAlertPatrol(EnemyAIController enemy, float proximityRange, float duration, float speed)
        {
            // begin at last known position target
            var targetX = enemy.GetLastKnownPlayerPosition().x;
            var targetPos = new Vector2(targetX, enemy.patrolY);
            enemy.MoveTowards(targetPos, speed);
            
            // once reached, start coroutine for patrolling
            enemy.StartCoroutine(AlertPatrolCoroutine(enemy, proximityRange, duration, speed, targetX));

        }
        
        private static IEnumerator AlertPatrolCoroutine(EnemyAIController enemy, float proximityRange, float duration, float speed, float lasttargetX)
        {
            var light2D = enemy.GetComponentInChildren<Light2D>();
            if (light2D != null)
                light2D.enabled = true;

            var elapsed = 0f;
            var centerX = enemy.GetLastKnownPlayerPosition().x;
            var toRight = true;
            var leftBound = centerX - proximityRange;
            var rightBound = centerX + proximityRange;

            while (elapsed < duration)
            {
                float targetX = toRight ? rightBound : leftBound;
                Vector2 patrolPos = new Vector2(targetX, enemy.patrolY);
                if (Mathf.Abs(enemy.transform.position.x - targetX) > 0.1f)
                {
                    enemy.MoveTowards(patrolPos, speed);
                }
                else
                {
                    toRight = !toRight;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (light2D != null)
                light2D.enabled = false;
        }
    }
}

