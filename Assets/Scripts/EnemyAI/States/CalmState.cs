// Assets/Scripts/EnemyAI/States/CalmState.cs
using System.Linq;
using UnityEngine;
using static EnemyUtils.EnemyUtils;
using UnityEngine.Rendering.Universal;


namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/CalmState")]
    public class CalmState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Calm;
        [Header("Ranges & Speeds")]
        public float noiseDetectionRange = 5f;
        [Header("Group Conversation")]
        [Tooltip("If >0, two Calm enemies within this X-distance and on-screen will stop.")]
        [SerializeField] private float conversationProximityRange = 2f;
        [Tooltip("Seconds to converse before resuming patrol")]
        [SerializeField] private float conversationDuration = 10f;
        private const float PatrolThreshold = 0.1f;
        private const float TalkChance = 0.5f;

        public void EnterState(EnemyAIController enemy)
        {
            var light2D = enemy.GetComponentInChildren<Light2D>();
            if (light2D != null)
                light2D.enabled = false;
            // Initialize conversation and patrol
            enemy.StopMovement();
            enemy.currentPatrolIndex    = 0;
            enemy.isConversing          = false;
            enemy.conversationCompleted = false;
            enemy.conversationTimer     = conversationDuration;
        }

        public void UpdateState(EnemyAIController enemy)
        {
            var dt = Time.deltaTime;
            // 1) check first if player in range and not hiding to move into chase mode
            EnemyEnterChaseModeIfNeeded(enemy);
            // 2) check if there is another calm enemy in close for conversation and talk to them
            var random = Random.Range(0f, 1f);
            if (random<TalkChance && TryHandleConversation(enemy, conversationProximityRange, conversationDuration, dt))
                return;
            
            // 2) Patrol on X-axis
            HandlePatrol(enemy, enemy.patrolPointsX, enemy.patrolY, enemy.calmMoveSpeed, PatrolThreshold);
        }

        public void ExitState(EnemyAIController enemy)
        {
            enemy.StopMovement();
        }
        
        // ------------------ Implementing Listener from interface in calm state ------------------ //
        public void OnNoiseRaised(Vector2 noisePosition, EnemyAIController enemy)
        {
            if (Vector2.Distance(enemy.transform.position, noisePosition) <= noiseDetectionRange
                && !enemy.IsPlayerHiding())
            {
                enemy.lastKnownNoisePosition = noisePosition;
                enemy.ChangeState(enemy.searchingState);
            }
        }
    }
}
