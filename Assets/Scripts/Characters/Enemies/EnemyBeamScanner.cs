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

        private void Awake()
        {
            patrol = GetComponent<EnemyPatrolController>();
            if (!beamPivot) Debug.LogError("BeamPivot not assigned!", this);
            if (!detectionCollider) detectionCollider = beamPivot.GetComponent<PolygonCollider2D>();

            if (sweepLeft != 0f || sweepRight != 0f)
            {
                StartCoroutine(SweepRoutine());
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

            while (true)
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
        }

        public void PlayerSpotted(GameObject player)
        {
            Debug.Log(alarmMessage, player);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
