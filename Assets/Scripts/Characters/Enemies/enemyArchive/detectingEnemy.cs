using System;
using UnityEngine;

namespace Characters.Enemies
{
    public class DetectingEnemy : MonoBehaviour
    {
        public float viewDistance = 5f;
        public float viewAngle = 45f;
        public LayerMask playerMask;
        public LayerMask obstacleMask;
        private FieldOfView fieldOfView;
        private GameObject player;
        [SerializeField] private float fov = 90f;
        [SerializeField] private Transform pfFieldOfView;

        private void Start()
        {
            Debug.Log(fieldOfView);
            fieldOfView = Instantiate(pfFieldOfView, null).GetComponent<FieldOfView>();
            Debug.Log(fieldOfView);
            fieldOfView.SetFoV(fov);
            fieldOfView.SetViewDistance(viewDistance);
        }

        private void Update()
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, viewDistance, playerMask);
            if (playerCollider != null)
            {
                Transform player1 = playerCollider.transform;
                Vector2 directionToPlayer = (player1.position - transform.position).normalized;
                float angle = Vector2.Angle(transform.right, directionToPlayer);

                if (angle < viewAngle / 2f)
                {
                    float distanceToPlayer = Vector2.Distance(transform.position, player1.position);

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask);
                    if (hit.collider == null)
                    {
                        Debug.Log("Player detected!");
                        // Trigger alert, chase, etc.
                    }
                }
            }
            if (fieldOfView != null) {
                fieldOfView.SetOrigin(transform.position);
                fieldOfView.SetAimDirection(GetAimDir());
            }

            Debug.DrawLine(transform.position, transform.position + GetAimDir() * 10f);
        }
        private void FindTargetPlayer()
        {
            playerMoveAxis _playerMove = player.GetComponent<playerMoveAxis>();
            if (Vector3.Distance(GetPosition(), _playerMove.GetPosition()) < viewDistance) {
                // Player inside viewDistance
                Vector3 dirToPlayer = (_playerMove.GetPosition() - GetPosition()).normalized;
                if (Vector3.Angle(GetAimDir(), dirToPlayer) < fov / 2f) {
                    // Player inside Field of View
                    RaycastHit2D raycastHit2D = Physics2D.Raycast(GetPosition(), dirToPlayer, viewDistance);
                    if (raycastHit2D.collider != null) {
                        // Hit something
                        if (raycastHit2D.collider.gameObject.GetComponent<playerMoveAxis>() != null) {
                            // Hit Player
                            Debug.Log("Player detected!");
                        } else {
                            // Hit something else
                        }
                    }
                }
            }
        }
        
        public Vector3 GetPosition() {
            return transform.position;
        }

        public Vector3 GetAimDir() {
            return Vector3.left;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, viewDistance);

            Vector3 rightBoundary = Quaternion.Euler(0, 0, viewAngle / 2f) * transform.right;
            Vector3 leftBoundary = Quaternion.Euler(0, 0, -viewAngle / 2f) * transform.right;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);
            Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        }
    }
}