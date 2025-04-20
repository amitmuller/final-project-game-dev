using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Stats")]
    public float detectionRange = 10f;
    public float attackRadius = 1.5f;
    public float attackCooldown = 2f;
    public int health = 100;
    public float speed = 2f;

    private float lastAttackTime = Mathf.NegativeInfinity;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        Debug.unityLogger.Log(player.position + " : " + distance);
        Debug.DrawRay(transform.position, player.position - transform.position, Color.red);
        if (distance < detectionRange)
        {
            Debug.Log(distance);
            // Debug.DrawRay(transform.position, player.position - transform.position, Color.red);
            GoToPlayer();
        }

        if (distance < attackRadius && Time.time - lastAttackTime > attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
            Die();
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    protected abstract void GoToPlayer();
    protected abstract void Attack();
}