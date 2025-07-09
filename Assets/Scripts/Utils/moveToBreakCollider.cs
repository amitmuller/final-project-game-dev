using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class moveToBreakCollider: MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene(2);
            }
        }
    }
}