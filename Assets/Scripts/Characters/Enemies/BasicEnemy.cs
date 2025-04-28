using System;
using UnityEngine;

public class BasicEnemy : Enemy
{
    private bool hasAttacked = false;
    [SerializeField] private Vector2 initialPosition  = Vector2.zero;

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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = initialPosition;
        }
    }
}