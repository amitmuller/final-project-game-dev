// Assets/Scripts/EnemyAI/States/CalmState.cs
using System.Linq;
using static CalmStateUtils.CalmStateUtils;
using UnityEngine;
using static EnemyUtils.EnemyUtils;
using UnityEngine.Rendering.Universal;
using static NoiseManager;
using Unity.VisualScripting;

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
        [Tooltip("Seconds the enemy will wait after reaching every waypoint, before moving to next one")]
        public float idleTime = 2f;

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

            // 1) check first if player in range and not hiding to move into chase mode
            EnemyEnterChaseModeIfNeeded(enemy);

            // 2) Handle idle between patrol waypoints
            if (enemy.IsIdle)
            {
                enemy.IdleTimer += Time.deltaTime;
                // Switching idle off if the timer is up, and there are patrol points set (it means the enemy is not infinitely idling)
                if ((idleTime < enemy.IdleTimer) && 
                    (enemy.patrolPoints.Length > 0))
                {
                    enemy.IdleTimer = 0f;
                    enemy.IsIdle = false;
                }
                return;
            }

            // 3) Patrol on X-axis, only if there are patrol points set
            if (enemy.patrolPoints == null)
            {
                Debug.LogError("Patrol points are not set for enemy: " + enemy.name);
            }

            // Moving to the next waypoint, if there are any waypoints
            if (enemy.patrolPoints.Length >= 1 && 
                HandlePatrol(
                    enemy, enemy.patrolPoints[enemy.currentPatrolIndex], enemy.patrolY, enemy.calmMoveSpeed, PatrolThreshold, idleTime))
            {
                enemy.IsIdle = true;
                enemy.currentPatrolIndex = (enemy.currentPatrolIndex + 1) % enemy.patrolPoints.Length;
            }
            // If there are no patrol points, enemy will remain in idle state
            else if (enemy.patrolPoints.Length == 0) 
            {
                enemy.IsIdle = true;
            }
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
