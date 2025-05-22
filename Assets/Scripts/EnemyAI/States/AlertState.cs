// AlertState.cs
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
                // only chase if player still visible and not hiding
                bool playerIsHiding = enemy.playerHideScript != null
                                      && enemy.playerHideScript.IsHiding();
                float distanceToPlayer = Vector2.Distance(
                    enemy.transform.position,
                    enemy.playerTransform.position
                );
                bool canSeePlayer = !playerIsHiding
                                    && distanceToPlayer <= enemy.detectionRange;

                if (canSeePlayer)
                {
                    enemy.ChangeState(enemy.chaseState);
                }
                else
                {
                    enemy.lastKnownNoisePosition = 
                        enemy.playerTransform.position;
                    enemy.ChangeState(enemy.searchingState);
                }
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            // nothing special
        }
    }
}