using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

using EnemyAI;

namespace AlertStateUtils
{
    public static class AlertStateUtils
    {
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
        public static void HandleAlertPatrol(EnemyAIController enemy, float proximityRange, float speed)
        {
            // if a patrol coroutine is already running, do nothing
            if (enemy.isAlertPatrolling)
                return;
            enemy.isAlertPatrolling = true;
            enemy.StartCoroutine(AlertPatrolCoroutine(enemy, proximityRange, speed));

        }
        
        private static IEnumerator AlertPatrolCoroutine(EnemyAIController enemy, float range, float speed)
        {

            var centerX  = enemy.GetLastKnownPlayerPosition().x;
            var  toRight  = true;
            var leftX    = centerX - range;
            var rightX   = centerX + range;

            // loop while this enemy stays in Alert
            while (enemy.CurrentStateType == EnemyStateType.Alert)
            {
                var targetX = toRight ? rightX : leftX;
                Vector2 targetPos = new Vector2(targetX, enemy.patrolY);

                if (Mathf.Abs(enemy.transform.position.x - targetX) > 0.1f)
                    enemy.MoveTowards(targetPos, speed);
                else
                    toRight = !toRight;          // bounce at edge

                yield return null;
            }
            
            enemy.isAlertPatrolling = false;
        }
    
        
        public static void AlertNearbyEnemies(EnemyAIController source, float radius)
        {
            foreach (var other in EnemyAIController.AllEnemies)
            {
                if (other == source) continue;                     // skip self
                if (other.CurrentStateType == EnemyStateType.Alert) continue; // already alert
                if (Vector2.Distance(source.transform.position, other.transform.position) > radius) continue;

                other.lastKnownNoisePosition = source.GetLastKnownPlayerPosition();
                other.ChangeState(other.alertState);               // pull neighbour into Alert
            }
        }
    }
}