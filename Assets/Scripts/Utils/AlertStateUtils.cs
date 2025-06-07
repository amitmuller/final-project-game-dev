using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

using EnemyAI;


public class AlertStateUtils
{
    /// <summary>
    /// After alert timer expires, transition to Chase if the player is visible;
    /// otherwise set lastKnownNoisePosition and switch to Searching.
    /// </summary>
    public void HandleAlertGoingToLastKnownPlayerPosition(EnemyAIController enemy)
    {
        enemy.StartCoroutine(GoToLastKnownPlayerPositionToStartAlertPatrol(enemy, 1.5f));
    }
    
    /// <summary>
    /// Patrol around last known noise position within a given proximity for a duration,
    /// enabling the flashlight during the patrol.
    /// </summary>
    public void HandleAlertPatrol(EnemyAIController enemy, float proximityRange, float speed)
    {
        // if a patrol coroutine is already running, do nothing
        if (enemy.isAlertPatrolling) return;
        enemy.isAlertPatrolling = true;
        enemy.StartCoroutine(AlertPatrolCoroutine(enemy, proximityRange, speed));

    }

    private static IEnumerator GoToLastKnownPlayerPositionToStartAlertPatrol(EnemyAIController enemy, float speed)
    {
        var target = enemy.GetLastKnownPlayerPosition();
        while (Mathf.Abs(target.x - enemy.transform.position.x) > 1f)
        {
            enemy.MoveTowards(target, speed);
            yield return null;
        }
        enemy.isGoingToStarAlertPatrolling = false;
        enemy.StopMovement();
    }
    
    private static IEnumerator AlertPatrolCoroutine(EnemyAIController enemy, float range, float speed)
    {

        var centerX  = enemy.transform.position.x;
        var  toRight  = true;
        var leftX    = centerX - range;
        var rightX   = centerX + range;

        // loop while this enemy stays in Alert
        while (enemy.CurrentStateType == EnemyStateType.Alert)
        {
            var targetX = toRight ? rightX : leftX;
            Vector2 targetPos = new Vector2(targetX, enemy.patrolY);

            if (Mathf.Abs(enemy.transform.position.x - targetX) > 0.1f)
                enemy.MoveTowards(targetPos, speed);
            else
                toRight = !toRight;          // bounce at edge

            yield return null;
        }
        
        enemy.isAlertPatrolling = false;
    }

    
    public void AlertNearbyEnemies(EnemyAIController source, float radius)
    {
        foreach (var other in EnemyAIController.AllEnemies)
        {
            Debug.Log("enemy count: " + EnemyAIController.AllEnemies.Count);
            if (other == source) continue;                     // skip self
            if (other.CurrentStateType == EnemyStateType.Alert || other.CurrentStateType == EnemyStateType.Chase) continue;
            if (Mathf.Abs(source.transform.position.x - other.transform.position.x) > radius) continue;
            other.ChangeState(other.alertState);               // pull neighbour into Alert
        }
    }
}
