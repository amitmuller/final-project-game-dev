using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Characters.Enemies
{
    public class EnemyBeamScanner : MonoBehaviour
    {
        [Header("Beam")]
        [SerializeField] private Transform beamPivot;
        [SerializeField] private PolygonCollider2D detectionCollider;

        [Header("Sweep Settings")]
        [Range(-90, 0)] public float sweepLeft = -20f;
        [Range(0, 90)] public float sweepRight = 20f;
        public float sweepDuration = 3f;
        public float pauseMin = 0.4f;
        public float pauseMax = 1.0f;

        [Header("Detection")]
        public string alarmMessage = "⚠️  Player Spotted!";
    
        private EnemyPatrolController patrol;
        private float sweepOffset = 0f;
        private bool sweepingRight = true;
        private Coroutine sweepRoutine;
        [SerializeField] private float lookinDelay = 2f;


        private void Awake()
        {
            patrol = GetComponent<EnemyPatrolController>();
            if (!beamPivot) Debug.LogError("BeamPivot not assigned!", this);
            if (!detectionCollider) detectionCollider = beamPivot.GetComponent<PolygonCollider2D>();

            if (sweepLeft != 0f || sweepRight != 0f)
            {
                sweepRoutine = StartCoroutine(SweepRoutine());
            }
            float fwd = patrol != null && patrol.FacingRight ? 180f : 0f;
            beamPivot.localRotation = Quaternion.Euler(0, 0, fwd + sweepOffset);
        }

        private void Update()
        {
            // Update beam facing every frame based on patrol direction
            float fwd = patrol != null && patrol.FacingRight ? 180f : 0f;
            beamPivot.localRotation = Quaternion.Euler(0, 0, fwd + sweepOffset);
        }

        private IEnumerator SweepRoutine()
        {
            sweepOffset = sweepLeft;
            sweepingRight = true;
            float fwdOverride = -1f;

            while (!patrol.holding)
            {
                float target = sweepingRight ? sweepRight : sweepLeft;
                float sweepSpeed = Mathf.Abs(sweepRight - sweepLeft) / sweepDuration;

                sweepOffset = Mathf.MoveTowards(sweepOffset, target, sweepSpeed * Time.deltaTime);

                if (Mathf.Approximately(sweepOffset, target))
                {
                    yield return new WaitForSeconds(Random.Range(pauseMin, pauseMax));
                    sweepingRight = !sweepingRight;
                }
                else
                {
                    yield return null;
                }
            }
            
            while (patrol.holding)
            {
                float target = sweepingRight ? sweepRight : sweepLeft;
                float sweepSpeed = Mathf.Abs(sweepRight - sweepLeft) / sweepDuration;

                // Occasionally randomize facing direction (0 or 180)
                if (Random.value < 0.5f)
                    fwdOverride = Random.value < 0.5f ? 0f : 180f;

                // Smoothly rotate toward target offset
                while (!Mathf.Approximately(sweepOffset, target) && patrol.holding)
                {
                    sweepOffset = Mathf.MoveTowards(sweepOffset, target, sweepSpeed * Time.deltaTime);

                    float fwd = fwdOverride >= 0f
                        ? fwdOverride
                        : (patrol != null && patrol.FacingRight ? 180f : 0f);

                    beamPivot.localRotation = Quaternion.Euler(0, 0, fwd + sweepOffset);
                    yield return null;
                }

                // Wait a bit before flipping again
                yield return new WaitForSeconds(Random.Range(pauseMin + 1f, pauseMax + 2f));

                sweepingRight = !sweepingRight;
            }
            
            sweepRoutine = StartCoroutine(SweepRoutine());

        }

        public void PlayerSpotted(GameObject player)
        {
            Debug.Log(alarmMessage, player);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public void ReactToSound(Vector3 soundSource)
        {
            if (sweepRoutine != null)
            {
                StopCoroutine(sweepRoutine);
                sweepRoutine = null;
            }

            StartCoroutine(LookTowardSoundRoutine(soundSource));
        }
        
        private IEnumerator LookTowardSoundRoutine(Vector3 soundSource)
        {
            Vector3 direction = soundSource - beamPivot.position;

            // Slightly bias the direction upward (e.g. +0.5 in Y)
            direction.y += 0.5f;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            beamPivot.localRotation = Quaternion.Euler(0, 0, angle);
            Debug.Log($"{name} heard a noise and is looking (slightly up) at {soundSource}");

            yield return new WaitForSeconds(lookinDelay);

            sweepRoutine = StartCoroutine(SweepRoutine());
        }




    }
}
