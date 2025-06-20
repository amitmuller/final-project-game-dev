using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

using EnemyAI;


public class AlertStateUtils
{
    // ------------------------------- HANDLERS ------------------------------- //
    public void AlertNearbyEnemies(EnemyAIController source, float radius)
    {
        foreach (var other in EnemyAIController.AllEnemies)
        {
            if (other == source) continue;                     // skip self
            if (other.CurrentStateType == EnemyStateType.Alert || other.CurrentStateType == EnemyStateType.Chase) continue;
            if (Mathf.Abs(source.transform.position.x - other.transform.position.x) > radius) continue;
            other.ChangeState(other.alertState);               // pull neighbour into Alert
        }
    }
    
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
    
    
    // ------------------------------- Coroutines ------------------------------- //
    
    
    private static IEnumerator GoToLastKnownPlayerPositionToStartAlertPatrol(EnemyAIController enemy, float speed)
    {
        const float range = 1f;
        const float maxDuration = 5f;   // bail-out time
        
        // Cache only the X component of the target
        var targetX = enemy.GetLastKnownPlayerPosition().x;
        var timer = 0f;

        // Loop until we’re within `range` on the X axis
        while (Mathf.Abs(enemy.transform.position.x - targetX) > range)
        {
            // Move only along x
            var pos = enemy.transform.position;
            var newX = Mathf.MoveTowards(pos.x, targetX, speed * Time.deltaTime);
            enemy.transform.position = new Vector3(newX, pos.y, pos.z);

            yield return null;
            timer += Time.deltaTime;

            if (timer > maxDuration)
            {
                Debug.LogWarning("GoToLastKnownPlayerPositionToStartAlertPatrol: timed out before reaching target");
                break;
            }
        }
        enemy.isGoingToStarAlertPatrolling = false;
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
            var targetPos = new Vector2(targetX, enemy.patrolY);

            if (Mathf.Abs(enemy.transform.position.x - targetX) > 0.1f)
                enemy.MoveTowards(targetPos, speed);
            else
                toRight = !toRight;          // bounce at edge

            yield return null;
        }
        
        enemy.isAlertPatrolling = false;
    }
}
