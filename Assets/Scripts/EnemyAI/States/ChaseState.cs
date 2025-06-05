// Assets/Scripts/EnemyAI/States/ChaseState.cs
using UnityEngine;
using static ChaseStateUtils.ChaseStateUtils;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/ChaseState")]
    public class ChaseState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Chase;

        public void EnterState(EnemyAIController enemy)
        {
            // todo play chase animation here
        }

        public void UpdateState(EnemyAIController enemy)
        {
            //  Abort chase immediately if player is hiding
            if (enemy.IsPlayerHiding())
            {
                // Record last known player position, then switch to alert
                enemy.lastKnownNoisePosition = enemy.playerTransform.position;
                enemy.ChangeState(enemy.alertState);
                return;
            }
            NearbyEnemiesTransitionToChase(enemy,7f);
            // Pursue the player
            enemy.MoveTowards(enemy.playerTransform.position, enemy.chaseMoveSpeed);
            
        }

        public void ExitState(EnemyAIController enemy)
        {
            // todo stop chase animation here
        }
    }
}