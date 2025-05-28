using UnityEngine;
using static EnemyUtils.EnemyUtils;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/AlertState")]
    public class AlertState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Alert;
        private const float ProximityPatrolRange = 5f;
        private const float PatrolAlertSpeed = 1f;
        public void EnterState(EnemyAIController enemy)
        {
            enemy.StopMovement();
            enemy.alertTimer = enemy.alertDuration;
        }

        public void UpdateState(EnemyAIController enemy)
        {
            enemy.alertTimer -= Time.deltaTime;
            if (enemy.alertTimer <= 0f)
                HandleAlertTransition(enemy); // moving to needed state base on player
            else
            {
                HandleAlertPatrol(enemy, ProximityPatrolRange, enemy.alertDuration-3f, PatrolAlertSpeed); // patroling near last known plaayr pos
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            // pass
        }
    }
}