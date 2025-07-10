// Assets/Scripts/EnemyAI/States/CalmState.cs
using System.Linq;
using static CalmStateUtils.CalmStateUtils;
using UnityEngine;
using static EnemyUtils.EnemyUtils;
using UnityEngine.Rendering.Universal;
using static NoiseManager;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/CalmState")]
    public class CalmState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Calm;
        [Header("Ranges & Speeds")]
        public float noiseDetectionRange = 15f;
        [Header("Group Conversation")]
        [Tooltip("If >0, two Calm enemies within this X-distance and on-screen will stop.")]
        [SerializeField] private float conversationProximityRange = 2f;
        [Tooltip("Seconds to converse before resuming patrol")]
        [SerializeField] private float conversationDuration = 10f;
        private const float PatrolThreshold = 1f;

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
            var dt = Time.deltaTime;
            // 1) check first if player in range and not hiding to move into chase mode
            EnemyEnterChaseModeIfNeeded(enemy);

            // 2) Patrol on X-axis
            HandlePatrol(enemy, enemy.patrolPoints, enemy.patrolY, enemy.calmMoveSpeed, PatrolThreshold);
        }

        public void ExitState(EnemyAIController enemy)
        {
            enemy.prevState = EnemyStateType.Calm;
            enemy.StopMovement();
        }
        
        //------------------ Implementing Listener from interface in calm state ------------------ //
        public void OnNoiseRaised(Vector2 noisePosition, EnemyAIController enemy)
        {
            if (enemy.CurrentStateType != EnemyStateType.Calm) return;
            // Debug.Log("noise raised on calm state distance from noise: " + 
            //           Vector2.Distance(enemy.transform.position, noisePosition) + 
            //           "noise detection range is: " +noiseDetectionRange);
            if (Vector2.Distance(enemy.transform.position, noisePosition) <= noiseDetectionRange)
            {
                Debug.Log("inside noise range");
                enemy.lastKnownNoisePosition = noisePosition;
                enemy.ChangeState(enemy.searchingState);
            }
        }
    }
}
