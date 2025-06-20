using UnityEngine;
using UnityEngine.Rendering.Universal;
using static EnemyUtils.EnemyUtils;
using static NoiseManager;
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
            
            // 2) if enemy is alert he will alert his friend in proximity
            alertUtils.AlertNearbyEnemies(enemy, enemy.spreadRadius);
            
            // 3) if is needed go into search state
            const float noiseStaleDuration = 2f;
            if (Time.time - LastNoiseTime <= noiseStaleDuration
                && Vector2.Distance(enemy.transform.position, LastNoisePosition) <= noiseDetectionRange)
            {
                enemy.lastKnownNoisePosition = LastNoisePosition;
                enemy.StopMovement();
                enemy.ChangeState(enemy.searchingState);
                return;
            }
            if (enemy.CurrentStateType == EnemyStateType.Chase) return;
            
            // 4) Otherwise patrol across alert patrol radius
            Debug.Log($"{enemy.name} going to last position = " + enemy.isGoingToStarAlertPatrolling + " enemy patrolling = " + enemy.isAlertPatrolling);
            if (enemy.isGoingToStarAlertPatrolling)
            {
                alertUtils.HandleAlertGoingToLastKnownPlayerPosition(enemy);
                return;
            }
            alertUtils.HandleAlertPatrol(enemy, enemy.alertPatrolRadius, enemy.alertSpeed);
        }

        public void ExitState(EnemyAIController enemy)
        {
            enemy.prevState = EnemyStateType.Alert;
            enemy.StopAllCoroutines();
            enemy.QuesitonIconSwitch(false);
        }
        
        // ------------------ Implementing Listener from interface in Alert state ------------------ //
       
        public void OnNoiseRaised(Vector2 noisePosition, EnemyAIController enemy)
        {
            if (enemy.CurrentStateType != EnemyStateType.Alert) return;
            Debug.Log("noise raised on alert state distance from noise: " + 
                      Vector2.Distance(enemy.transform.position, noisePosition) + 
                      "noise detection range is: " +noiseDetectionRange);
            if (Vector2.Distance(enemy.transform.position, noisePosition) <= noiseDetectionRange)
            {
                Debug.Log("inside noise range");
                enemy.lastKnownNoisePosition = noisePosition;
                enemy.ChangeState(enemy.searchingState);
            }
        }
        
    }
}