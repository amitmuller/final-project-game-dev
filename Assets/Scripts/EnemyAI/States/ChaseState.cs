// Assets/Scripts/EnemyAI/States/ChaseState.cs

using Unity.VisualScripting;
using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "AI States/ChaseState")]
    public class ChaseState : ScriptableObject, IEnemyState
    {
        public EnemyStateType StateType => EnemyStateType.Chase;

        private GameObject _icon;

        public void EnterState(EnemyAIController enemy)
        {
            // todo play chase animation here
            turnOnIconIfNeeded(enemy, true);

        }

        public void UpdateState(EnemyAIController enemy)
        {
            Debug.Log("player hide: " + enemy.IsPlayerHiding());
            //  Abort chase immediately if player is hiding
            if (enemy.IsPlayerHiding())
            {
                // Record last known player position, then switch to alert
                enemy.lastKnownNoisePosition = enemy.playerTransform.position;
                enemy.ChangeState(enemy.alertState);
                return;
            }

            // Pursue the player
            enemy.MoveTowards(enemy.playerTransform.position, enemy.chaseMoveSpeed);

            // If player goes out of sight or range, switch to Searching
            var distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);

            var stillInSight = !enemy.IsPlayerHiding() && distanceToPlayer <= enemy.detectionRange;

            if (!stillInSight)
            {
                enemy.lastKnownNoisePosition = enemy.playerTransform.position;
                enemy.ChangeState(enemy.alertState);
            }
        }

        public void ExitState(EnemyAIController enemy)
        {
            // todo stop chase animation here
            turnOnIconIfNeeded(enemy, false);
        }

        private void turnOnIconIfNeeded(EnemyAIController enemy, bool turnOn)
        {
            if (_icon != null)
            {
                _icon.GameObject().SetActive(turnOn);
            }
            else
            {
                var prefab = Resources.Load<GameObject>("Exclamation Mark");
                Debug.Log(prefab);
                if (prefab)
                {
                    _icon = Instantiate(prefab);
                    _icon.transform.localPosition = new Vector3(enemy.transform.localPosition.x,
                        enemy.transform.localPosition.y + 2.5f, 0f);
                }
            }
        }
    }
}