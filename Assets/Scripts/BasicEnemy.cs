using UnityEngine;

public class BasicEnemy : Enemy
{
    private bool hasAttacked = false;

    protected override void GoToPlayer()
    {
        Debug.Log("Going to player");
        if (hasAttacked) return;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    protected override void Attack()
    {
        if (hasAttacked) return;

        Debug.Log("BasicEnemy attacks the player!");
        hasAttacked = true;

        Die();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange/2);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius/2);
    }
}