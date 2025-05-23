// Assets/Scripts/EnemyAI/States/CalmState.cs
using System.Linq;
using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/CalmState")]
    public class CalmState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Calm;

        [Header("Group Conversation")]
        [Tooltip("If >0, two Calm enemies within this X-distance and on-screen will stop.")]
        [SerializeField] private float conversationProximityRange = 2f;
        [Tooltip("Seconds to converse before resuming patrol")]
        [SerializeField] private float conversationDuration = 10f;
        private const float PatrolThreshold = 0.1f;

        public void EnterState(EnemyAIController enemy)
        {
            // Initialize conversation and patrol
            enemy.StopMovement();
            enemy.currentPatrolIndex    = 0;
            enemy.isConversing          = false;
            enemy.conversationCompleted = false;
            enemy.conversationTimer     = conversationDuration;
        }

        public void UpdateState(EnemyAIController enemy)
        {
            float dt = Time.deltaTime;

            // 0) Group‐stop & conversation
            if (conversationProximityRange > 0f)
            {
                bool groupNearby = EnemyAIController
                    .AllEnemies
                    .Where(e => e != enemy && e.CurrentStateType == EnemyStateType.Calm)
                    .Any(e =>
                        Mathf.Abs(enemy.transform.position.x - e.transform.position.x) <= conversationProximityRange
                        && enemy.IsVisibleOnCamera()
                        && e.IsVisibleOnCamera()
                    );

                if (groupNearby && !enemy.conversationCompleted)
                {
                    if (!enemy.isConversing)
                    {
                        // Start the conversation timer
                        enemy.isConversing      = true;
                        enemy.conversationTimer = conversationDuration;
                    }

                    // During conversation, stand still
                    enemy.conversationTimer -= dt;
                    enemy.StopMovement();

                    // End conversation after duration
                    if (enemy.conversationTimer <= 0f)
                    {
                        enemy.isConversing          = false;
                        enemy.conversationCompleted = true;
                    }
                    return;
                }
                else if (!groupNearby)
                {
                    // Reset so future meetings can trigger again
                    enemy.conversationCompleted = false;
                }
            }

            // 1) Immediate player detection → Chase
            bool playerHidden  = enemy.IsPlayerHiding();
            float distToPlayer = Vector2.Distance(
                enemy.transform.position,
                enemy.playerTransform.position
            );
            if (!playerHidden && distToPlayer <= enemy.detectionRange)
            {
                enemy.ChangeState(enemy.chaseState);
                return;
            }

            // 2) Patrol on X-axis
            if (enemy.patrolPointsX != null && enemy.patrolPointsX.Length > 0)
            {
                float targetX     = enemy.patrolPointsX[enemy.currentPatrolIndex];
                Vector2 targetPos = new Vector2(targetX, enemy.patrolY);
                enemy.MoveTowards(targetPos, enemy.calmMoveSpeed);

                // Advance to next point when close
                if (Mathf.Abs(enemy.transform.position.x - targetX) < PatrolThreshold)
                {
                    enemy.currentPatrolIndex =
                        (enemy.currentPatrolIndex + 1) % enemy.patrolPointsX.Length;
                }
            }
            else
            {
                enemy.StopMovement();
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            enemy.StopMovement();
        }
    }
}
