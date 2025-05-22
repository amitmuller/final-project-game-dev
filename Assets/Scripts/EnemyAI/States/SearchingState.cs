// Assets/Scripts/EnemyAI/States/SearchingState.cs
using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/SearchingState")]
    public class SearchingState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Searching;

        public void EnterState(EnemyAIController enemy)
        {
            // Start the search timer
            enemy.searchTimer = enemy.searchDuration;
            // Stop any existing movement
            enemy.StopMovement();
        }

        public void UpdateState(EnemyAIController enemy)
        {
            // 1) Move toward the last known noise spot, but only along X
            float targetX = enemy.lastKnownNoisePosition.x;
            Vector2 targetPosition = new Vector2(targetX, enemy.patrolY);

            if (Mathf.Abs(enemy.transform.position.x - targetX) > 0.1f)
            {
                enemy.MoveTowards(targetPosition, enemy.searchMoveSpeed);
            }
            else
            {
                enemy.StopMovement();
            }
            // 2) Timer expired → return to Calm
            enemy.searchTimer -= Time.deltaTime;
            if (enemy.searchTimer <= 0f)
            {
                enemy.ChangeState(enemy.calmState);
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            // pass
        }
    }
}