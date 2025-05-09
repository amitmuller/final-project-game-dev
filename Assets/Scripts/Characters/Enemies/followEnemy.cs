using System;
using UnityEngine;

namespace Characters.Enemies
{
    public class followEnemy : Enemy
    {
        protected override void GoToPlayer()
        {
            // Debug.Log("Going to player");
            // if (hasAttacked) return;

            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
        
        protected override void Attack()
        {
            // if (hasAttacked) return;

            // Debug.Log("BasicEnemy attacks the player!");
            // hasAttacked = true;

            Die();
        }
        
        
        
    }
}