using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/AlertState")]
    public class AlertState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Alert;

        public void EnterState(EnemyAIController enemy)
        {
            enemy.StopMovement();
            enemy.alertTimer = enemy.alertDuration;
        }

        public void UpdateState(EnemyAIController enemy)
        {
            enemy.alertTimer -= Time.deltaTime;
            if (enemy.alertTimer <= 0f)
            {
                // check if player is both visible and not hiding
                bool playerHidden      = enemy.IsPlayerHiding();
                float distanceToPlayer = Vector2.Distance(
                    enemy.transform.position,
                    enemy.playerTransform.position
                );
                bool canSeePlayer = !playerHidden
                                    && distanceToPlayer <= enemy.detectionRange;

                if (canSeePlayer)
                {
                    enemy.ChangeState(enemy.chaseState);
                }
                else
                {
                    // lost sight → start searching at last known location
                    enemy.lastKnownNoisePosition = enemy.playerTransform.position;
                    enemy.ChangeState(enemy.searchingState);
                }
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            // pass
        }
    }
}