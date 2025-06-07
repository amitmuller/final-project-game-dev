// Assets/Scripts/EnemyAI/States/ChaseState.cs

using MoreMountains.Tools;
using UnityEngine;
using DG.Tweening;
using static ChaseStateUtils.ChaseStateUtils;
using Vector2 = System.Numerics.Vector2;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/ChaseState")]
    public class ChaseState : ScriptableObject, IEnemyState
    {
        private  const float CHASE_SPREAD = 8f;
        public EnemyStateType StateType => EnemyStateType.Chase;

        public void EnterState(EnemyAIController enemy)
        {
            // todo play chase animation here
        }

        public void UpdateState(EnemyAIController enemy)
        {
            //  Abort chase immediately if player is hiding
            if (enemy.IsPlayerHiding())
            {
                // Record last known player position, then switch to alert
                enemy.lastKnownNoisePosition = enemy.playerTransform.position;
                enemy.ChangeState(enemy.alertState);
                return;
            }
            NearbyEnemiesTransitionToChase(enemy,CHASE_SPREAD);
            // Pursue the player
            if (Mathf.Abs(enemy.transform.position.x - enemy.playerTransform.position.x) < 3f)
            {
                enemy.transform.DOMoveX(enemy.playerTransform.position.x,3f).SetEase(Ease.OutQuint);
            }
            else
            {
                enemy.MoveTowards(enemy.playerTransform.position, enemy.chaseMoveSpeed);
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            DOTween.KillAll();
        }
    }
}