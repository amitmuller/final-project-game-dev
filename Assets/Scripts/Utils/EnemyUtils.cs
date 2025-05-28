using System;
using UnityEngine;
using EnemyAI;
using System.Linq;


namespace EnemyUtils
{
    public static class EnemyUtils
    {
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
        public static bool IsAnyCalmEnemyNearby(EnemyAIController self, float proximityRange)
        {
            return EnemyAIController.AllEnemies
                .Where(e => e != self && e.CurrentStateType == EnemyStateType.Calm)
                .Any(e =>
                    Mathf.Abs(self.transform.position.x - e.transform.position.x) <= proximityRange
                    && self.IsVisibleOnCamera()
                    && e.IsVisibleOnCamera()
                );
        }
    }
    }
}

