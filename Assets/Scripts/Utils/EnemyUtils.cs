using System.Collections;
using System.Linq;
using UnityEngine;
using EnemyAI;
using Unity.VisualScripting.FullSerializer;
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
            var playerHidden = enemy.IsPlayerHiding();
            if (playerHidden)
            {
                return false;
            }

            var isPlayerInFront =
                (enemy.GetIsWalkingRight() && enemy.playerTransform.position.x < enemy.transform.position.x) ||
                (!enemy.GetIsWalkingRight() && enemy.playerTransform.position.x > enemy.transform.position.x);
            
            if (isPlayerInFront)
            {
                return true;
            }
            
            // if (!playerHidden && enemy.IsInChasingDistanceFromPlayer())
            // {
            //     Debug.Log("enemy.chaseState");
            //     enemy.ChangeState(enemy.chaseState);
            //     return true;
            // }
            return false;
        }
        
        
    }
}

