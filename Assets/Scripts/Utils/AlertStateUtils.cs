using UnityEngine;
using System.Collections;

using EnemyAI;


public static class AlertStateUtils
{
    // ------------------------------- HANDLERS ------------------------------- //
    public static void AlertNearbyEnemies(EnemyAIController source, float radius)
    {
        foreach (var other in EnemyAIController.AllEnemies)
        {
            if (other == source) continue;                     // skip self
            if (other.CurrentStateType == EnemyStateType.Alert || other.CurrentStateType == EnemyStateType.Chase) continue;
            if (Mathf.Abs(source.transform.position.x - other.transform.position.x) > radius) continue;
            other.ChangeState(other.alertState);               // pull neighbour into Alert
            Debug.Log(other.CurrentStateType + "WE SWITCHED THE NEXT TO IT ENEMY");
            
        }
    }
    
    /// <summary>
    /// Patrol around last known noise position within a given proximity for a duration,
    /// enabling the flashlight during the patrol.
    /// </summary>
    public static void HandleAlertPatrol(EnemyAIController enemy, float proximityRange, float speed)
    {
        // if a patrol coroutine is already running, do nothing
        Debug.Log($"[Enemy] {enemy.name} -> is alert patroling {enemy.isAlertPatrolling}");
        if (enemy.isAlertPatrolling) return;
        enemy.isAlertPatrolling = true;
        enemy.StartCoroutine(AlertPatrolCoroutine(enemy, proximityRange, speed));

    }
    
    
    // ------------------------------- Coroutines ------------------------------- //
    
    
    
    private static IEnumerator AlertPatrolCoroutine(EnemyAIController enemy, float range, float speed)
    {

        var centerX  = enemy.transform.position.x;
        var toRight = (enemy.GetIsWalkingRight()); 
        
        var leftX    = centerX - range;
        var rightX   = centerX + range;

        // loop while this enemy stays in Alert
        while (enemy.CurrentStateType == EnemyStateType.Alert)
        {
            var targetX = toRight ? rightX : leftX;
            var targetPos = new Vector2(targetX, enemy.patrolY);
            
            Vector2 targetVec = enemy.MoveTowards(targetPos, speed);
            if (Mathf.Abs(enemy.transform.position.x - targetVec.x) < 0.1f)
                toRight = !toRight;          // bounce at edge

            yield return null;
        }
        
        enemy.isAlertPatrolling = false;
    }
}
