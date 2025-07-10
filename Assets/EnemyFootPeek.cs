using System;
using UnityEngine;


public class EnemyFootPeek : MonoBehaviour
{
    private const int HiddenEnemyOrder = -100;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("enemy detected in show bubble");
            foreach (var enemy in EnemyAIController.AllEnemies){
                Debug.Log("DIS: "+ Mathf.Abs(enemy.transform.position.x - transform.position.x));
                if (Mathf.Abs(enemy.transform.position.x - transform.position.x) <= 5)
                {
                    enemy.RestoreSortingOrder();
                }
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("enemy out from show bubble");
            foreach (var enemy in EnemyAIController.AllEnemies)
                if (Mathf.Abs(enemy.transform.position.x - transform.position.x) <= 5 )
                {
                    enemy.SetSortingOrder(HiddenEnemyOrder);
                }
        }
    }
    
    
}
