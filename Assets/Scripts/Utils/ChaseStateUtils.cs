using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

using EnemyAI;

namespace ChaseStateUtils
{
    public static class ChaseStateUtils
    {
        public static void NearbyEnemiesTransitionToChase(EnemyAIController source, float radius)
        {
            foreach (var other in EnemyAIController.AllEnemies)
            {
                if (other == source || other.CurrentStateType == EnemyStateType.Chase) continue;     // skip 
                if (Mathf.Abs(source.transform.position.x - other.transform.position.x) > radius) continue;

                other.ChangeState(other.chaseState);
            }
        }
    }
}