using UnityEngine;

public class EnemyFOVTrigger : MonoBehaviour
{
    private EnemyAIController enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyAIController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player") && !enemy.IsPlayerHiding())
        {
            enemy.ChangeState(enemy.chaseState);
        }
    }
}