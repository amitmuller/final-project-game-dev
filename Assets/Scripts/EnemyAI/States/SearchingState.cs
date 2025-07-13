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
        private const float ArrivalThreshold = 1f;
        private const float MaxTimeForState = 8f;
        
        public EnemyStateType StateType => EnemyStateType.Searching;

        public void EnterState(EnemyAIController enemy)
        {
            AudioManager.Instance.PlayEffect("enemyConfused");
            

            enemy.searchTargetX = enemy.lastKnownNoisePosition.x;
            enemy.searchFirstTime = true;

            enemy.moveToNoiseTimer = 0f;
            enemy.searchTimer      = enemy.searchDuration;
            enemy.isStop = false;

            if (enemy.filledQuestionIcon != null)
            {
                enemy.filledQuestionIcon.fillAmount = 1f;
                enemy.filledQuestionIcon.gameObject.SetActive(true);
            }
            
            enemy.StopMovement();
        }

        public void UpdateState(EnemyAIController enemy)
        {
            
            EnemyEnterChaseModeIfNeeded(enemy);

            if (enemy.moveToNoiseTimer > MaxTimeForState)
            {
                if (enemy.prevState == EnemyStateType.Calm)
                    enemy.ChangeState(enemy.calmState);
                else
                    enemy.ChangeState(enemy.alertState);
            }
            enemy.moveToNoiseTimer += Time.deltaTime;
            var deltaX = Mathf.Abs(enemy.transform.position.x - enemy.searchTargetX);
            
            var stillMoving = deltaX > ArrivalThreshold && enemy.moveToNoiseTimer < MaxMoveTime;
            
            if (stillMoving)
            {
                // move only in X
                enemy.isStop = false;
                enemy.searchFirstTime = true;
                enemy.MoveTowards(new Vector2(enemy.searchTargetX, enemy.patrolY), enemy.searchMoveSpeed);
                Debug.Log("MOVE TO SEARCHING" + enemy.moveToNoiseTimer);
                return;
            }
            Debug.Log("enemy sotp: "+ enemy.isStop);
            enemy.isStop = true;
            if (enemy.searchFirstTime) enemy.UpdateAnimation();
            
            enemy.searchFirstTime = false;
            enemy.searchTimer -= Time.deltaTime;
            enemy.StopMovement();
            Debug.Log("enemy is searching in place: " + enemy.searchTimer);
            // when time’s up, go back to Calm or Alert
            if (enemy.searchTimer <= 0f)
            {
                enemy.isStop = false;
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
            enemy.prevState = EnemyStateType.Searching;
            enemy.filledQuestionIcon?.gameObject.SetActive(false);
        }
    }
}