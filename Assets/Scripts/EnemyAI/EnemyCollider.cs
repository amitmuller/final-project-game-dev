using System;
using Characters.Player;
using UnityEngine;

namespace EnemyAI
{
    public class EnemyCollider:MonoBehaviour
    {
        private PlayerHide playerHide;
        private void Awake()
        {
            var playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            if (playerTransform != null)
            {
                playerHide = playerTransform.GetComponent<PlayerHide>();
            }
        }
        public bool IsPlayerHiding(){
            return playerHide != null && playerHide.IsHiding();
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.CompareTag("Player") && !IsPlayerHiding())
            {
                // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                GameManager.Instance.checkpoint(collision.transform);
            }
        }
    }
}