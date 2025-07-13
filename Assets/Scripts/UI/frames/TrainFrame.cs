using System.Collections;
using UnityEngine;
using UI;

/// <summary>
/// A SequenceFrame that moves a train GameObject from its current local start position
/// to a target GameObject's position over its displayTime, and draws a gizmo at the destination.
/// </summary>
public class TrainFrame : SequenceFrame
{
    [Header("Train Movement")]
    [Tooltip("The train Transform to move")]        public Transform trainTransform;
    [Tooltip("Destination GameObject; train moves to this object's position")] public Transform destinationTransform;

    private void Reset()
    {
        // Initialize frame displayTime if unset
        if (displayTime <= 0)
            displayTime = 1f;
    }

    /// <summary>
    /// Draw a gizmo sphere at the destination position in world space.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (destinationTransform == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(destinationTransform.position, 0.2f);
    }

    public override void PlayFrame()
    {
        if (trainTransform == null || destinationTransform == null)
        {
            Debug.LogWarning("TrainFrame: missing trainTransform or destinationTransform");
            return;
        }

        // Capture the current local start position
        Vector3 startLocal = trainTransform.localPosition;
        // Compute end in local space relative to this frame's transform
        Vector3 worldEnd = destinationTransform.position;
        Vector3 endLocal = transform.InverseTransformPoint(worldEnd);

        // Begin moving over displayTime
        StartCoroutine(MoveTrain(startLocal, endLocal));
    }

    private IEnumerator MoveTrain(Vector3 startLocal, Vector3 endLocal)
    {
        float elapsed = 0f;
        while (elapsed < displayTime)
        {
            float t = elapsed / displayTime;
            trainTransform.localPosition = Vector3.Lerp(startLocal, endLocal, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        // Ensure final position
        trainTransform.localPosition = endLocal;
    }
}