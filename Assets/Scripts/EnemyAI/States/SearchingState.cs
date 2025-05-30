// Assets/Scripts/EnemyAI/States/SearchingState.cs
using UnityEngine;
using static SerchingStateUtils.SerchingStateUtils;
using UnityEngine.Rendering.Universal;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/SearchingState")]
    public class SearchingState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Searching;

        public void EnterState(EnemyAIController enemy)
        {
            // Reset timer when state begins
            enemy.searchTimer = enemy.searchDuration;
            enemy.StopMovement();
        }

        public void UpdateState(EnemyAIController enemy)
        {
            var targetX = enemy.lastKnownNoisePosition.x;
            var targetPosition = new Vector2(targetX, enemy.patrolY);
            // moving towords sound last pos
            if (Mathf.Abs(enemy.transform.position.x - targetX) > 0.1f)
            {
                enemy.MoveTowards(targetPosition, enemy.searchMoveSpeed);
            }
            else
            {
                enemy.StopMovement();

                // Only start search in place once, when timer is at full value
                if (Mathf.Approximately(enemy.searchTimer, enemy.searchDuration))
                {
                    SearchInNoisePosition(enemy, enemy.searchDuration);
                }

                // Only count down after reaching the spot
                enemy.searchTimer -= Time.deltaTime;
                if (enemy.searchTimer <= 0f)
                {
                    enemy.ChangeState(enemy.calmState);
                }
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            var light2D = enemy.GetComponentInChildren<Light2D>();
            light2D.enabled = false;
            enemy.StopAllCoroutines();
        }
    }
}