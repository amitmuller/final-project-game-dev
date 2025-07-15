// Assets/Scripts/EnemyAI/States/CalmState.cs
using System.Linq;
using static CalmStateUtils.CalmStateUtils;
using UnityEngine;
using static EnemyUtils.EnemyUtils;
using UnityEngine.Rendering.Universal;
using static NoiseManager;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;

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

            if (enemy.patrolPoints == null)
            {
                Debug.LogError("Patrol points are not set for enemy: " + enemy.name);
            }

            // If there are no patrol points, enemy will remain in idle state
            if (enemy.patrolPoints.Length == 0)
            {
                Debug.Log($"Enemy {enemy.name} entering permanent idle");
                enemy.IsPermanentIdle = true;
                if (Mathf.Abs(enemy.transform.position.x - enemy.InitialPosition.x) > PatrolThreshold)
                {
                    Debug.Log($"Enemy {enemy.name} - Returning to initial position");
                    enemy.IsReturningToInitial = true;
                    enemy.IsPermanentIdle = false;
                }
            }
        }

        public void UpdateState(EnemyAIController enemy)
        {

            // 1) check first if player in range and not hiding to move into chase mode
            EnemyEnterChaseModeIfNeeded(enemy);

            // If the enemy is in permanent idle state and is far from
            // its initial position, then move the enemy back to its original position
            //if (enemy.IsPermanentIdle && enemy.IsReturningToInitial)
            if (enemy.IsReturningToInitial)
            {
                if (HandlePatrol(enemy, enemy.InitialPosition.x, enemy.patrolY, enemy.calmMoveSpeed, PatrolThreshold))
                {
                    enemy.IsReturningToInitial = false;
                    enemy.IsPermanentIdle = true;
                }
                return;
            }

            // 2) Handle idle between patrol waypoints
            if (enemy.IsPatrolIdle)
            {
                enemy.PatrolIdleTimer += Time.deltaTime;
                // Switching idle off if the timer is up, and there are patrol points set (it means the enemy is not infinitely idling)
                if ((idleTime < enemy.PatrolIdleTimer) && 
                    (enemy.patrolPoints.Length > 0))
                {
                    enemy.PatrolIdleTimer = 0f;
                    enemy.IsPatrolIdle = false;
                }
                return;
            }

            // 3) Patrol on X-axis, only if there are patrol points set

            // Moving to the next waypoint, if there are any waypoints
            if (enemy.patrolPoints.Length >= 1 && 
                HandlePatrol(
                    enemy, enemy.patrolPoints[enemy.currentPatrolIndex].position.x, enemy.patrolY, enemy.calmMoveSpeed, PatrolThreshold))
            {
                enemy.IsPatrolIdle = true;
                enemy.currentPatrolIndex = (enemy.currentPatrolIndex + 1) % enemy.patrolPoints.Length;
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
                enemy.lastKnownNoisePosition = noisePosition;
                enemy.ChangeState(enemy.searchingState);
            }
        }
    }
}
