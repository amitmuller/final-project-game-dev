using UnityEngine;
using UnityEngine.Rendering.Universal;
using static EnemyUtils.EnemyUtils;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/AlertState")]
    public class AlertState : ScriptableObject, IEnemyState
    {
        private AlertStateUtils alertUtils = new AlertStateUtils();
        public float noiseDetectionRange = 5f;
        public EnemyStateType StateType => EnemyStateType.Alert;
        public void EnterState(EnemyAIController enemy)
        {
            enemy.StopMovement();
            enemy.isGoingToStarAlertPatrolling = true;
            enemy.isAlertPatrolling = false;
            enemy.QuesitonIconSwitch(true);
        }

        public void UpdateState(EnemyAIController enemy)
        {
            // 1) If player visible and not hiding → switch to Chase
            if (EnemyEnterChaseModeIfNeeded(enemy)) return;
            
            alertUtils.AlertNearbyEnemies(enemy, enemy.spreadRadius);
            
            if (enemy.CurrentStateType == EnemyStateType.Chase) return;
            Debug.Log($"{enemy.name} going to last position = " + enemy.isGoingToStarAlertPatrolling + " enemy patrolling = " + enemy.isAlertPatrolling);
            if (enemy.isGoingToStarAlertPatrolling)
            {
                alertUtils.HandleAlertGoingToLastKnownPlayerPosition(enemy);
                return;
            }
            // 2) Otherwise patrol indefinitely across alertPatrolRadius
            alertUtils.HandleAlertPatrol(enemy, enemy.alertPatrolRadius, enemy.alertSpeed);
        }

        public void ExitState(EnemyAIController enemy)
        {
            enemy.StopAllCoroutines();
            enemy.QuesitonIconSwitch(false);
        }
        public void OnNoiseRaised(Vector2 noisePosition, EnemyAIController enemy)
        {
            if (Vector2.Distance(enemy.transform.position, noisePosition) <= noiseDetectionRange)
            {
                enemy.lastKnownNoisePosition = noisePosition;
                enemy.ChangeState(enemy.searchingState);
            }
        }
    }
}