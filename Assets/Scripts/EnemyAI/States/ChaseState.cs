// Assets/Scripts/EnemyAI/States/ChaseState.cs
using UnityEngine;

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
                // Record last known player position, then switch to searching
                enemy.lastKnownNoisePosition = enemy.playerTransform.position;
                enemy.ChangeState(enemy.searchingState);
                return;
            }

            // Pursue the player
            enemy.MoveTowards(enemy.playerTransform.position, enemy.chaseMoveSpeed);

            // If player goes out of sight or range, switch to Searching
            float distanceToPlayer = Vector2.Distance(
                enemy.transform.position,
                enemy.playerTransform.position
            );

            bool stillInSight = !enemy.IsPlayerHiding()
                                && distanceToPlayer <= enemy.detectionRange;

            if (!stillInSight)
            {
                enemy.lastKnownNoisePosition = enemy.playerTransform.position;
                enemy.ChangeState(enemy.searchingState);
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            // todo stop chase animation here
        }
    }
}