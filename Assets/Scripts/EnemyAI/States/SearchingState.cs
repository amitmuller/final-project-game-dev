// Assets/Scripts/EnemyAI/States/SearchingState.cs
using UnityEngine;
using static SerchingStateUtils.SerchingStateUtils;
using static EnemyUtils.EnemyUtils;
using UnityEngine.Rendering.Universal;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/SearchingState")]
    public class SearchingState : ScriptableObject, IEnemyState
    {
        private const float MaxMoveTime = 3f;
        public EnemyStateType StateType => EnemyStateType.Searching;

        public void EnterState(EnemyAIController enemy)
        {
            // Reset timer when state begins
            if (enemy.filledQuestionIcon != null)
            {
                enemy.filledQuestionIcon.fillAmount = 1f;
                enemy.filledQuestionIcon.gameObject.SetActive(true);
            }
            enemy.searchTimer = enemy.searchDuration;
            enemy.moveToNoiseTimer = 0f;
            Debug.Log("search timer"+ enemy.searchTimer);
            enemy.StopMovement();
        }

        public void UpdateState(EnemyAIController enemy)
        {
            // some vars
            var targetX = enemy.lastKnownNoisePosition.x;
            var deltaX  = Mathf.Abs(enemy.transform.position.x - targetX);
            var targetPosition = new Vector2(targetX, enemy.patrolY);
            if (EnemyEnterChaseModeIfNeeded(enemy)) return;
            
            // moving towords sound last pos
            if (deltaX > 0.5f && enemy.moveToNoiseTimer < MaxMoveTime)
            {
                enemy.MoveTowards(new Vector2(targetX, enemy.patrolY),
                    enemy.searchMoveSpeed);

                enemy.moveToNoiseTimer += Time.deltaTime;
                Debug.Log($"Moving to noise movind time: {enemy.moveToNoiseTimer}");
                return;
            }
            
            enemy.StopMovement();

            // Only count down after reaching the spot
            enemy.searchTimer -= Time.deltaTime;
            Debug.Log("search timer in noise position is: " +enemy.searchTimer);
            if (enemy.filledQuestionIcon != null)
            {
                var fillPercent = (enemy.searchTimer / enemy.searchDuration);
                enemy.filledQuestionIcon.fillAmount = Mathf.Clamp01(fillPercent);
            }
            
            if (enemy.searchTimer <= 0f)
            {
                if (enemy.prevState == EnemyStateType.Calm)
                {
                    enemy.ChangeState(enemy.calmState);
                }
                else
                {
                    enemy.ChangeState(enemy.alertState);
                }
                
            }
        
        }

        public void ExitState(EnemyAIController enemy)
        {
            enemy.prevState = EnemyStateType.Searching;
            enemy.StopAllCoroutines();
            enemy.filledQuestionIcon.gameObject.SetActive(false);
        }
    }
}