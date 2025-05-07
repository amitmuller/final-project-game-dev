
using Unity.VisualScripting;
using UnityEngine;

public class ShootingEnemy : Enemy
{
    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    // [SerializeField] private Transform firePoint;
    [SerializeField] private float shootInterval = 3f;

    private float shootTimer;

    protected override void Update()
    {
        base.Update(); // Keeps movement and melee attack behavior from base class

        // // Handle shooting every few seconds
        // if (player == null) return;
        //
        // shootTimer += Time.deltaTime;
        // if (shootTimer >= shootInterval && base.canAttack)
        // {
        //     shootTimer = 0f;
        //     
        // }
    }

    protected override void GoToPlayer()
    {
        // Optional: disable movement if using a stationary shooter
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    protected override void Attack()
    {
        // Optional: keep melee or disable this if shooting only
        Debug.Log("BasicEnemy performed melee attack!");
        // player.position = player.position + Vector3.right * 2; // Example knockback effect
        ShootAtPlayer();
    }

    private void ShootAtPlayer()
    {
        Debug.Log("shoot");
        if (projectilePrefab == null || player == null) return;

        Vector2 shootDirection = (player.position-transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<EnemyProjectile>().Initialize(new Vector2(shootDirection.x,shootDirection.y));
    }

}

