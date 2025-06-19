
using System.Linq;
using UnityEngine;
using EnemyAI;


namespace CalmStateUtils
{
    public static class CalmStateUtils
    {
        
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
                    && self.IsInChasingDistanceFromPlayer()
                    && e.IsInChasingDistanceFromPlayer()
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
            var nearby = AllEnemiesNearby(self, proximityRange);
            if (nearby && !self.conversationCompleted)
            {
                if (!self.isConversing)
                {
                    EnemyAIController.ConversationEncounterCount ++;
                    
                    if (EnemyAIController.ConversationEncounterCount % 5 == 0)
                    {
                        self.isConversing      = true;
                        self.conversationTimer = conversationDuration;
                    }
                    else
                    {
                        return false;
                    }
                }

                // If isConversing==true, stand still and timer
                self.conversationTimer -= deltaTime;
                self.StopMovement();
                
                if (self.conversationTimer <= 0f)
                {
                    self.isConversing          = false;
                    self.conversationCompleted = true;
                }

                return true; 
            }
            
            if (!nearby)
            {
                self.conversationCompleted = false;
            }

            return false;
        }

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
            if (patrolPointsX == null)
            {
                return;
            }

            if (patrolPointsX.Length == 1)
            {
                // peek very large number outside of screen -> moving one point
                var target = patrolPointsX[0];
                var moveTo = new Vector2(target,patrolY);
                self.MoveTowards(moveTo, speed);
            }
                
            else if (patrolPointsX.Length > 1)
            {
               var target = patrolPointsX[self.currentPatrolIndex];
               var moveTo = new Vector2(target, patrolY);
                var rb = self.GetComponent<Rigidbody2D>();
                self.MoveTowards(moveTo, speed);
                if (Mathf.Abs(self.transform.position.x - moveTo.x) < threshold)
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
    }
}