using System.Collections;
using System.Linq;
using UnityEngine;
using EnemyAI;
using UnityEngine.Rendering.Universal;

namespace EnemyUtils
{
    public static class EnemyUtils
    {
        
        /// <summary>
        /// changes the state into chase mode if needed
        /// </summary>
        /// <param name="enemy"></param>
        public static void EnemyEnterChaseModeIfNeeded(EnemyAIController enemy)
        {
            bool isPlayerInFront =
                (enemy.getIsWalkingRight() && enemy.playerTransform.position.x < enemy.transform.position.x) ||
                (!enemy.getIsWalkingRight() && enemy.playerTransform.position.x > enemy.transform.position.x);

            if (isPlayerInFront)
            {
                return;
            }
            
            var playerHidden  = enemy.IsPlayerHiding();
            var distToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);
            if (!playerHidden && distToPlayer <= enemy.detectionRange)
            {
                enemy.ChangeState(enemy.chaseState);
            }
        }
        
        
    }
}

