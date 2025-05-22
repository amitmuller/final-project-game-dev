// SearchingState.cs
using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/SearchingState")]
    public class SearchingState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Searching;

        public void EnterState(EnemyAIController enemy)
        {
            enemy.searchTimer = enemy.searchDuration;
        }

        public void UpdateState(EnemyAIController enemy)
        {
            // Walk to last known noise/player position
            if (enemy.lastKnownNoisePosition != Vector2.zero)
            {
                float distance = Vector2.Distance(
                    enemy.transform.position,
                    enemy.lastKnownNoisePosition
                );
                if (distance > 0.1f)
                {
                    enemy.MoveTowards(
                        enemy.lastKnownNoisePosition,
                        enemy.searchMoveSpeed
                    );
                }
                else
                {
                    enemy.StopMovement();
                }
            }

            // If player reappears and not hiding, chase them again
            bool playerIsHiding = enemy.playerHideScript != null
                                  && enemy.playerHideScript.IsHiding();
            float distanceToPlayer = Vector2.Distance(
                enemy.transform.position,
                enemy.playerTransform.position
            );
            if (!playerIsHiding
                && distanceToPlayer <= enemy.detectionRange)
            {
                enemy.ChangeState(enemy.chaseState);
                return;
            }

            // Give up after timer
            enemy.searchTimer -= Time.deltaTime;
            if (enemy.searchTimer <= 0f)
            {
                enemy.ChangeState(enemy.calmState);
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            // nothing special
        }
    }
}