using UnityEngine;
using static AlertStateUtils.AlertStateUtils;
using UnityEngine.Rendering.Universal;
using static EnemyUtils.EnemyUtils;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/AlertState")]
    public class AlertState : ScriptableObject, IEnemyState
    {

        public EnemyStateType StateType => EnemyStateType.Alert;
        public void EnterState(EnemyAIController enemy)
        {
            enemy.StopMovement();
            AlertNearbyEnemies(enemy, enemy.spreadRadius);
            enemy.isAlertPatrolling = false;   
        }

        public void UpdateState(EnemyAIController enemy)
        {
            // 1) If player visible and not hiding → switch to Chase
            EnemyEnterChaseModeIfNeeded(enemy);
            AlertNearbyEnemies(enemy, enemy.spreadRadius);
            if (enemy.CurrentStateType == EnemyStateType.Chase)
                return;   

            // 2) Otherwise patrol indefinitely across alertPatrolRadius
            HandleAlertPatrol(enemy, enemy.alertPatrolRadius, enemy.alertSpeed);
        }

        public void ExitState(EnemyAIController enemy)
        {
            var light2D = enemy.GetComponentInChildren<Light2D>();
            light2D.enabled = false;
            enemy.StopAllCoroutines();
        }
    }
}