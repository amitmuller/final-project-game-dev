using UnityEngine;
using DG.Tweening;
using static ChaseStateUtils.ChaseStateUtils;
using static EnemyUtils.EnemyUtils;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/ChaseState")]
    public class ChaseState : ScriptableObject, IEnemyState
    {
        private const float CHASE_SPREAD = 8f;
        private const float DASH_WINDOW = 3f;

        // This holds our active tween (if any)
        private Tweener _dashTween;

        public EnemyStateType StateType => EnemyStateType.Chase;

        public void EnterState(EnemyAIController enemy)
        {
            // reset any previous tween
            _dashTween?.Kill();
            _dashTween = null;
            enemy.StopMovement();
            enemy.ExclamationIconSwitch(true);
        }

        public void UpdateState(EnemyAIController enemy)
        {
            // 1) If player hides, switch immediately
            if (enemy.IsPlayerHiding())
            {
                Debug.Log("got here");
                // kill only this transform’s tweens
                DOTween.Kill(enemy.transform);
                _dashTween.Kill();
                _dashTween = null;
                // clear any physics velocity as well
                enemy.StopMovement();

                // then switch
                enemy.ChangeState(enemy.alertState);
                return;
            }
            
            // 2) make your friends chase to
            NearbyEnemiesTransitionToChase(enemy, CHASE_SPREAD);

            var playerX = enemy.playerTransform.position.x;
            var dx      = Mathf.Abs(enemy.transform.position.x - playerX);

            // 3)  within dash‐window fire or update dash tween
            if (dx < DASH_WINDOW)
            {
                if (_dashTween == null || !_dashTween.IsActive())
                {
                    // compute how fast we want to dash
                    var dashSpeed = enemy.chaseDashSpeed > 0 ? enemy.chaseDashSpeed : enemy.chaseMoveSpeed * 2f;

                    // compute duration so that duration = distance / speed
                    var duration = dx / dashSpeed;
                    duration = Mathf.Max(duration, 0.05f);

                    // kill any stray tweens on this transform
                    DOTween.Kill(enemy.transform);

                    // start one‐shot dash reset _dashTween on complete
                    _dashTween = enemy.transform
                        .DOMoveX(playerX, duration)
                        .SetEase(Ease.OutQuint)
                        .OnComplete(() => { _dashTween = null; });
                }
            }
            // 4) Otherwise keep walking normally
            else
            {
                if (_dashTween != null)
                {
                    _dashTween.Kill();
                    _dashTween = null;
                }

                enemy.MoveTowards(enemy.playerTransform.position, enemy.chaseMoveSpeed);
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            enemy.prevState = EnemyStateType.Chase;

            // clean up any running dash
            _dashTween?.Kill();
            DOTween.Kill(enemy.transform);
            _dashTween = null;
            enemy.StopMovement();
            enemy.ExclamationIconSwitch(false);
        }
    }
}