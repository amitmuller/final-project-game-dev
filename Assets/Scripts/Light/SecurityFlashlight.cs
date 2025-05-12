using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Characters.Player;

public class SecurityFlashlight : MonoBehaviour
{
    /* ─────────────  hierarchy  ───────────── */
    [Header("Hierarchy")]
    [Tooltip("Child transform that carries the Light2D + collider.")]
    [SerializeField] private Transform beamPivot;

    [Tooltip("Polygon trigger on the same BeamPivot.")]
    [SerializeField] private PolygonCollider2D detectionCollider;

    /* ─────────────  sweep around forward dir  ───────────── */
    [Header("Sweep (around forward)")]
    [Range(-90, 0)]   [SerializeField] private float sweepLeft  = -20f;  // left of fwd
    [Range(0,  90)]   [SerializeField] private float sweepRight =  20f;  // right of fwd
    [Range(0.5f, 10f)][SerializeField] private float sweepSecs  =   3f;
    [SerializeField] private float pauseMin = 0.4f;
    [SerializeField] private float pauseMax = 1.0f;

    /* ─────────────  patrol movement  ───────────── */
    [Header("Patrol (translation)")]
    [SerializeField] private bool startAtRightEdge = true;    // ← start at rightEdgeX or leftEdgeX
    [SerializeField] private bool startMovingRight = false;  
    [SerializeField] private bool enablePatrol = true;
    [Range(-10, 0)]   [SerializeField] private float leftEdgeX  = -4f;
    [Range(0, 100)]    [SerializeField] private float rightEdgeX =  4f;
    [Range(0.5f, 10f)][SerializeField] private float walkSecs   =   4f;

    /* ─────────────  player spotting  ───────────── */
    [Header("Player")]
    [SerializeField] private string alarmMessage = "⚠️  Guard spotted the player!";


    private bool facingRight = false;   // updated by patrol


    private void Awake()
    {
        if (!beamPivot) Debug.LogError("BeamPivot not assigned!", this);
        if (!detectionCollider) detectionCollider = beamPivot.GetComponent<PolygonCollider2D>();

        // POSITION the guard based on config
        float startX = startAtRightEdge ? rightEdgeX : leftEdgeX;
        transform.localPosition = new Vector3(startX, transform.localPosition.y, transform.localPosition.z);

        // FACE the correct way before anything starts
        facingRight = startMovingRight;
        UpdateBeamPivotFacing();

        StartCoroutine(SweepRoutine());
        if (enablePatrol) StartCoroutine(PatrolRoutine());
    }


    private IEnumerator PatrolRoutine()
    {
        bool toRight = startMovingRight;
        Vector3 basePos = transform.localPosition;

        while (true)
        {
            float startX = toRight ? leftEdgeX : rightEdgeX;
            float endX   = toRight ? rightEdgeX : leftEdgeX;

            facingRight = toRight;
            UpdateBeamPivotFacing();  // instant torch flip

            for (float t = 0; t < 1f; t += Time.deltaTime / walkSecs)
            {
                float x = Mathf.Lerp(startX, endX, t);
                transform.localPosition = new Vector3(x, basePos.y, basePos.z);
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(pauseMin, pauseMax));
            toRight = !toRight;
        }
    }


    private IEnumerator SweepRoutine()
    {
        float offset = sweepLeft;            // current offset from forward
        bool dir = true;                     // dir == true → from left→right offset, false → back

        while (true)
        {
            /* recompute forward on **every** frame */
            float fwd = facingRight ? 180f : 0f;   // ← 180 = look right, 0 = look left


            /* move offset toward the other bound */
            float target = dir ? sweepRight : sweepLeft;
            offset = Mathf.MoveTowards(offset, target, Time.deltaTime * Mathf.Abs(sweepRight - sweepLeft) / sweepSecs);

            /* apply */
            beamPivot.localRotation = Quaternion.Euler(0, 0, fwd + offset);

            /* reached end? → pause + flip direction */
            if (Mathf.Approximately(offset, target))
            {
                yield return new WaitForSeconds(Random.Range(pauseMin, pauseMax));
                dir = !dir;
            }
            else
                yield return null;
        }
    }


    private void UpdateBeamPivotFacing()
    {
        beamPivot.localRotation = Quaternion.Euler(0, 0, facingRight ? 180f : 0f);
    }
    
    
    public void PlayerSpotted(GameObject player)
    {
        Debug.Log(alarmMessage, player);
        // TODO: raise alarm
    }
}
