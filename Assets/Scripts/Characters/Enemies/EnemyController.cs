using UnityEngine;

namespace Characters.Enemies
{
    public class EnemyPatrolController : MonoBehaviour
    {
        [SerializeField] private EnemyMovementData movementData;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Vector3 startPoint;
        private Vector3 endPoint;
        private bool goingForward;
        private bool hasStartedMoving;
        private bool isHolding;
        private float timer;

        public bool FacingRight { get; private set; }

        private void Start()
        {
            if (movementData == null)
            {
                Debug.LogError("Movement data not assigned", this);
                enabled = false;
                return;
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                if (!spriteRenderer)
                    Debug.LogWarning("No SpriteRenderer found for flipping!", this);
            }

            startPoint = transform.position;
            endPoint = startPoint + (Vector3)(movementData.direction.normalized * movementData.moveDistance);
            goingForward = true;

            // Set initial facing direction based on movement vector
            FacingRight = movementData.direction.x >= 0f;
            UpdateSpriteFacing();

            if (movementData.movementType == MovementType.OneDirection)
            {
                isHolding = true;
                timer = movementData.holdTimeAtStart;
                hasStartedMoving = false;
            }
        }

        private void Update()
        {
            if (movementData.movementType == MovementType.Static)
                return;

            if (movementData.movementType == MovementType.OneDirection)
            {
                HandleOneDirection();
                return;
            }

            HandleBackAndForth();
        }

        private void HandleOneDirection()
        {
            if (!hasStartedMoving)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    hasStartedMoving = true;
                }
                else
                {
                    return;
                }
            }

            Vector3 displacement = transform.position - startPoint;
            float distance = displacement.magnitude;

            if (distance < movementData.moveDistance)
            {
                transform.position += (Vector3)(movementData.direction.normalized * movementData.speed * Time.deltaTime);
            }
        }

        private void HandleBackAndForth()
        {
            if (isHolding)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                    isHolding = false;
                else
                    return;
            }

            Vector3 target = goingForward ? endPoint : startPoint;
            transform.position = Vector3.MoveTowards(transform.position, target, movementData.speed * Time.deltaTime);

            // Flip sprite if direction changes
            bool newFacingRight = (target - transform.position).x > 0f;
            if (newFacingRight != FacingRight)
            {
                FacingRight = newFacingRight;
                UpdateSpriteFacing();
            }

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                goingForward = !goingForward;
                isHolding = true;
                timer = goingForward ? movementData.holdTimeAtStart : movementData.holdTimeAtEnd;
            }
        }

        private void UpdateSpriteFacing()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !FacingRight;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (movementData == null) return;

            Gizmos.color = Color.yellow;
            Vector3 start = transform.position;
            Vector3 direction = movementData.direction.normalized;
            Vector3 end = start + direction * movementData.moveDistance;

            if (movementData.movementType != MovementType.Static)
            {
                Gizmos.DrawLine(start, end);
                Gizmos.DrawWireSphere(start, 0.1f);
                Gizmos.DrawWireSphere(end, 0.1f);
            }
        }
    }
}
