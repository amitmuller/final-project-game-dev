using UnityEngine;
using static EnemyUtils.EnemyUtils;
using UnityEngine.Rendering.Universal;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/AlertState")]
    public class AlertState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Alert;
        private const float PatrolAlertSpeed = 1f;
        public void EnterState(EnemyAIController enemy)
        {
            enemy.StopMovement();
            enemy.alertTimer = enemy.alertDuration;
            enemy.isAlertPatrolling = false;   
        }

        public void UpdateState(EnemyAIController enemy)
        {
            enemy.alertTimer -= Time.deltaTime;
            if (enemy.alertTimer <= 0f)
                HandleAlertTransition(enemy); // moving to needed state base on player
            else
            {
                var proximityPatrolRange = Random.Range(7f,12f);
                HandleAlertPatrol(enemy, proximityPatrolRange, enemy.alertDuration-3f, PatrolAlertSpeed); // patroling near last known plaayr pos
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