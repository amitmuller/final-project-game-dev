// CalmState.cs
using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/CalmState")]
    public class CalmState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Calm;

        public void EnterState(EnemyAIController enemy)
        {
            enemy.StopMovement();
        }

        public void UpdateState(EnemyAIController enemy)
        {
            // If player is hiding, do not detect
            bool playerIsHiding = enemy.playerHideScript != null
                                  && enemy.playerHideScript.IsHiding();
            if (!playerIsHiding)
            {
                float distanceToPlayer = Vector2.Distance(
                    enemy.transform.position,
                    enemy.playerTransform.position
                );
                if (distanceToPlayer <= enemy.detectionRange)
                {
                    enemy.ChangeState(enemy.alertState);
                    return;
                }
            }

            // TODO: add patrol or idle behavior here
        }

        public void ExitState(EnemyAIController enemy)
        {
            // nothing special
        }
    }
}