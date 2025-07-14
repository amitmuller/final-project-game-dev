
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
        public static void HandlePatrol(EnemyAIController self, float[] patrolPointsX, float patrolY, float speed, float threshold)
        {
            if (patrolPointsX == null) return;

            if (patrolPointsX.Length == 1)
            {
                // very large number outside of screen -> moving one point
                var target = patrolPointsX[0];
                var moveTo = new Vector2(target,patrolY);
                self.MoveTowards(moveTo, speed);
            }
                
            else if (patrolPointsX.Length > 1)
            {
               var target = patrolPointsX[self.currentPatrolIndex];
               var moveTo = new Vector2(target, patrolY);
               self.MoveTowards(moveTo, speed);
               
               if (Mathf.Abs(self.transform.position.x - moveTo.x) < threshold)
               {
                    self.timerForTurnAround += Time.deltaTime;
                    if (self.timerForTurnAround < turnTimer)
                    {
                        self.StopMovement();
                        self.turnInCalm = true;
                        self.UpdateAnimation();
                    }
                    else
                    {
                        self.turnInCalm = false;
                        self.UpdateAnimation();
                        self.currentPatrolIndex = (self.currentPatrolIndex + 1) % patrolPointsX.Length;
                        self.timerForTurnAround = 0f;
                    }
                   
               }


            }
            else
            {
                self.StopMovement();
            }
        }
    }
}