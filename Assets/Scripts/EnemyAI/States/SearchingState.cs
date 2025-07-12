using UnityEngine;
using static SerchingStateUtils.SerchingStateUtils;
using static EnemyUtils.EnemyUtils;
using UnityEngine.Rendering.Universal;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/SearchingState")]
    public class SearchingState : ScriptableObject, IEnemyState
    {
        private const float MaxMoveTime = 3f;   // how long we try to move on X
        private const float ArrivalThreshold = 0.5f;

        // snapshot of where the noise was when we entered
        private float _targetX;
        private bool firstTime;
        public EnemyStateType StateType => EnemyStateType.Searching;

        public void EnterState(EnemyAIController enemy)
        {
            AudioManager.Instance.PlayEffect("enemyConfused");
            firstTime = true;
            // 1) Record the exact spot where the noise happened
            _targetX = enemy.lastKnownNoisePosition.x;

            // 2) Reset both timers
            enemy.moveToNoiseTimer = 0f;
            enemy.searchTimer      = enemy.searchDuration;

            // 3) Reset UI
            if (enemy.filledQuestionIcon != null)
            {
                enemy.filledQuestionIcon.fillAmount = 1f;
                enemy.filledQuestionIcon.gameObject.SetActive(true);
            }

            // 4) Stop any residual motion
            enemy.StopMovement();
        }

        public void UpdateState(EnemyAIController enemy)
        {
            // log every frame so you can watch this flood the console
            var deltaX = Mathf.Abs(enemy.transform.position.x - _targetX);
            
            // First: if the player suddenly becomes visible → bail into Chase
            // (but only after we’ve had our turn moving/timed out)
            var stillMoving = deltaX > ArrivalThreshold && enemy.moveToNoiseTimer < MaxMoveTime;
            
            if (!stillMoving && EnemyEnterChaseModeIfNeeded(enemy)) return;

            // If we haven’t yet closed to within 0.5 on X and haven’t timed out
            if (stillMoving)
            {
                // move only in X
                enemy.MoveTowards(new Vector2(_targetX, enemy.patrolY), enemy.searchMoveSpeed);

                enemy.moveToNoiseTimer += Time.deltaTime;
                return;
            }

            enemy.isStop = true;
            if (firstTime) enemy.UpdateAnimation();
            firstTime = false;
            // Otherwise: either we’ve arrived or we ran out of move-time.
            // Stop, and start our “look around” countdown
            enemy.searchTimer -= Time.deltaTime;
            enemy.StopMovement();
            
            // when time’s up, go back to Calm or Alert
            if (enemy.searchTimer <= 0f)
            {
                if (enemy.prevState == EnemyStateType.Calm)
                    enemy.ChangeState(enemy.calmState);
                else
                    enemy.ChangeState(enemy.alertState);
            }
            
            
            

            // update the question icon
            if (enemy.filledQuestionIcon != null)
            {
                enemy.filledQuestionIcon.fillAmount =
                    Mathf.Clamp01(enemy.searchTimer / enemy.searchDuration);
            }
           
        }

        public void ExitState(EnemyAIController enemy)
        {
            enemy.isStop = false;
            enemy.prevState = EnemyStateType.Searching;
            enemy.filledQuestionIcon?.gameObject.SetActive(false);
        }
    }
}