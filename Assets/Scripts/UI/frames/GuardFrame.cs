using System.Collections;
using UnityEngine;
using Spine.Unity;
using UI;
using UnityEngine.Serialization;

/// <summary>
/// A SequenceFrame that activates a skeleton, plays a walk animation for a set duration,
/// then switches to idle and optionally shows another GameObject.
/// </summary>
public class SkeletonFrame : SequenceFrame
{
    [Header("Skeleton Settings")]
    [Tooltip("Root GameObject of the skeleton (will be activated)")]
    public GameObject skeletonObject;
    [Tooltip("Spine SkeletonAnimation component on the skeletonObject")]    
    public SkeletonAnimation skeletonAnim;
    [Tooltip("Name of the walk animation clip")]    
    public string walkAnimation;
    [Tooltip("Name of the idle animation clip")]    
    public string idleAnimation;
    [Tooltip("How long to play the walk animation and move before switching to idle")]    
    public float walkDuration = 1f;

    [Header("Movement")]
    [Tooltip("Destination transform the skeleton will walk to (world space)")]    
    public Transform destinationTransform;

    [FormerlySerializedAs("objectToShow")]
    [Header("Optional Extra")]
    public GameObject Door;

    public override void PlayFrame()
    {
        // Activate skeleton and extra object
        if (skeletonObject != null)
            skeletonObject.SetActive(true);
        if (Door != null)
            Door.SetActive(true);

        // Play walk animation
        if (skeletonAnim != null && !string.IsNullOrEmpty(walkAnimation))
            skeletonAnim.state.SetAnimation(0, walkAnimation, true);

        // Begin walking movement and then idle switch
        StartCoroutine(WalkAndIdle());
    }

    private IEnumerator WalkAndIdle()
    {
        float elapsed = 0f;

        // Record start and end positions in world space
        Vector3 startPos = skeletonObject.transform.position;
        Vector3 endPos = destinationTransform != null ? destinationTransform.position : startPos;

        while (elapsed < walkDuration)
        {
            float t = Mathf.Clamp01(elapsed / walkDuration);
            // Move skeletonObject smoothly toward destination
            skeletonObject.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Ensure final position
        skeletonObject.transform.position = endPos;

        // Switch to idle animation
        if (skeletonAnim != null && !string.IsNullOrEmpty(idleAnimation))
            skeletonAnim.state.SetAnimation(0, idleAnimation, true);
    }
}
