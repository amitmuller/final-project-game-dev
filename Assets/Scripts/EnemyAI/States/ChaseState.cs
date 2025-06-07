// Assets/Scripts/EnemyAI/States/ChaseState.cs

using System;
using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/ChaseState")]
    public class ChaseState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Chase;
        public void EnterState(EnemyAIController enemy)
        {
            enemy.exclamationIconSwitch(true);
            // todo play chase animation here
            
        }

        public void UpdateState(EnemyAIController enemy)
        {
            Debug.Log("player hide: " + enemy.IsPlayerHiding());
            //  Abort chase immediately if player is hiding
            if (enemy.IsPlayerHiding())
            {
                // Record last known player position, then switch to alert
                enemy.lastKnownNoisePosition = enemy.playerTransform.position;
                enemy.ChangeState(enemy.alertState);
                return;
            }

            // Pursue the player
            enemy.MoveTowards(enemy.playerTransform.position, enemy.chaseMoveSpeed);

            // If player goes out of sight or range, switch to Searching
            var distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);

            var stillInSight = !enemy.IsPlayerHiding() && distanceToPlayer <= enemy.detectionRange;

            if (!stillInSight)
            {
                enemy.lastKnownNoisePosition = enemy.playerTransform.position;
                enemy.ChangeState(enemy.alertState);
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            enemy.exclamationIconSwitch(false);
            // todo stop chase animation here
        }
    }
}