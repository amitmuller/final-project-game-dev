using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{

[RequireComponent(typeof(Animation))]
public class AnimationSequencerLegacy : MonoBehaviour
{
    [Header("Animation Component")]
    [Tooltip("Legacy Animation component containing your clips")] 
    public Animation animationComponent;

    [Header("Sequence")]
    [Tooltip("List of AnimationClips to play in order")] 
    public List<AnimationClip> sequence;

    [Tooltip("Should the sequence loop when finished?")]
    public bool loopSequence = false;

    private int currentIndex = 0;
    private bool isPlaying = false;

    void Reset()
    {
        // Auto-assign Animation component
        animationComponent = GetComponent<Animation>();
    }

    /// <summary>
    /// Begin playing the configured AnimationClip sequence.
    /// </summary>
    public void StartSequence()
    {
        if (sequence == null || sequence.Count == 0 || animationComponent == null)
            return;

        currentIndex = 0;
        PlayCurrent();
    }

    void PlayCurrent()
    {
        AnimationClip clip = sequence[currentIndex];
        if (clip == null)
            return;

        // Ensure clip is added to the Animation component
        string clipName = clip.name;
        if (!animationComponent.GetClip(clipName))
        {
            animationComponent.AddClip(clip, clipName);
        }

        isPlaying = true;
        animationComponent.Play(clipName);
        StartCoroutine(WaitForClipEnd(clip.length));
    }

    IEnumerator WaitForClipEnd(float duration)
    {
        yield return new WaitForSeconds(duration);

        isPlaying = false;
        currentIndex++;

        if (currentIndex >= sequence.Count)
        {
            if (loopSequence)
                currentIndex = 0;
            else
                yield break;
        }

        PlayCurrent();
    }

    /// <summary>
    /// Stop any playing sequence immediately.
    /// </summary>
    public void StopSequence()
    {
        StopAllCoroutines();
        if (animationComponent != null)
            animationComponent.Stop();

        isPlaying = false;
    }

    /// <summary>
    /// Returns whether a sequence is currently playing.
    /// </summary>
    public bool IsPlaying()
    {
        return isPlaying;
    }
}

    
}