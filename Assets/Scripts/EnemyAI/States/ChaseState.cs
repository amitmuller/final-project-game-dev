// ChaseState.cs
using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/ChaseState")]
    public class ChaseState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Chase;

        public void EnterState(EnemyAIController enemy)
        {
            // e.g. play a run animation
        }

        public void UpdateState(EnemyAIController enemy)
        {
            // Move toward the player's current position
            enemy.MoveTowards(
                enemy.playerTransform.position,
                enemy.chaseMoveSpeed
            );

            // Lose player if they hide or go out of range
            bool playerIsHiding = enemy.playerHideScript != null
                                  && enemy.playerHideScript.IsHiding();
            float distanceToPlayer = Vector2.Distance(
                enemy.transform.position,
                enemy.playerTransform.position
            );
            bool stillInSight = !playerIsHiding
                                && distanceToPlayer <= enemy.detectionRange;

            if (!stillInSight)
            {
                enemy.lastKnownNoisePosition = 
                    enemy.playerTransform.position;
                enemy.ChangeState(enemy.searchingState);
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            // e.g. stop run animation
        }
    }
}