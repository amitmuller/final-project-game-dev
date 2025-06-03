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
        public static bool EnemyEnterChaseModeIfNeeded(EnemyAIController enemy)
        {
            bool isPlayerInFront =
                (enemy.getIsWalkingRight() && enemy.playerTransform.position.x < enemy.transform.position.x) ||
                (!enemy.getIsWalkingRight() && enemy.playerTransform.position.x > enemy.transform.position.x);
            
            if (isPlayerInFront)
            {
                return true;
            }
            
            var playerHidden  = enemy.IsPlayerHiding();
            var distToPlayer = Mathf.Abs(enemy.transform.position.x-enemy.playerTransform.position.x);
            if (!playerHidden && distToPlayer <= enemy.detectionRange)
            {
                enemy.ChangeState(enemy.chaseState);
                return true;
            }
            return false;
        }
        
        
    }
}

