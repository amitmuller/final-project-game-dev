
using System.Linq;
using UnityEngine;
using EnemyAI;


namespace CalmStateUtils
{
    public static class CalmStateUtils
    {

        private const float turnTimer = 2f;
        private static bool AllEnemiesNearby(EnemyAIController self, float range)
        {
            return EnemyAIController.AllEnemies
                .Where(e => e != self && e.CurrentStateType == EnemyStateType.Calm)
                .Any(e =>
                    Mathf.Abs(self.transform.position.x - e.transform.position.x) <= range
                    && self.IsInChasingDistanceFromPlayer()
                    && e.IsInChasingDistanceFromPlayer());
            
        }
        
        /// <summary>
        /// Handles X-axis patrol and index advancement.
        /// </summary>
        public static bool HandlePatrol(EnemyAIController self, float patrolPointX, float patrolY, float speed, float threshold, float idleTime)
        {
            var target = patrolPointX;
            var moveTo = new Vector2(target, patrolY);
            self.MoveTowards(moveTo, speed);

            if (Mathf.Abs(self.transform.position.x - moveTo.x) < threshold)
            {
                self.turnInCalm = true;
                self.UpdateAnimation();
                self.StopMovement();
                return true;
            }

            return false;
        }
    }
}