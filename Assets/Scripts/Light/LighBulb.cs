using System;
using UnityEngine;

namespace Light
{
    public class LighBulb: MonoBehaviour
    {
        [Header("Sound Alert Settings")]
        [SerializeField] private float hearingRadius = 10f;
        [SerializeField] private Color gizmoColor = new Color(1f, 0.5f, 0f, 0.25f);

        // private void Start()
        // {
        //     Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hearingRadius);
        //     foreach (var hit in hits)
        //     {
        //         var scanner = hit.GetComponent<Characters.Enemies.EnemyBeamScanner>();
        //         if (scanner != null)
        //         {
        //             scanner.ReactToSound(transform.position);
        //         }
        //     }
        // }

        public void AlertNearbyEnemies()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hearingRadius);
            Debug.Log("Nearby enemies found"+hits.Length);
            foreach (var hit in hits)
            {
                Debug.Log(hit.name);
                var scanner = hit.GetComponent<Characters.Enemies.EnemyBeamScanner>();
                if (scanner != null)
                {
                    Debug.Log("scanner");
                    scanner.ReactToSound(transform.position);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, hearingRadius);
        }
    }
}
