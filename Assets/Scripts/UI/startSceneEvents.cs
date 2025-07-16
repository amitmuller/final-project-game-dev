using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UI;

public class StartSceneEvents : MonoBehaviour
{
    [SerializeField]
    private CameraFade cameraFade;

    [Tooltip("One‐shot train sound effect")]
    [SerializeField] private AudioSource trainSource;
    [SerializeField] private AudioSource DinoSource;
    
    [Tooltip("One‐shot train sound effect")]
    [Header("Spine Clips")] public SkeletonAnimation skeletonAnimationGoon;
    
    
    
    public void OnTrain()
    {
        if (trainSource != null)
        {
            trainSource.loop = false;
            trainSource.Play();
        }
    }
    private void OnEnable()
    {
        if (skeletonAnimationGoon != null)
            skeletonAnimationGoon.AnimationState.Event += HandleSpineEvent;
    }

    private void OnDisable()
    {
        if (skeletonAnimationGoon != null)
            skeletonAnimationGoon.AnimationState.Event -= HandleSpineEvent;
    }

    private void HandleSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        print("event ");
        switch (e.Data.Name)
        {
            case "voice":
                OnSound();
                break;
        }
    }
    public void OnSound()
    {
        if (DinoSource != null)
        {
            DinoSource.loop = false;
            DinoSource.Play();
        }
    }
    
    public void OnGoonFinished()
    {
        var entry = skeletonAnimationGoon.state.SetAnimation(2, "idleSmoking", false);
        entry.Complete += e =>
        {
            skeletonAnimationGoon.state.SetEmptyAnimation(1, 0.1f);
            cameraFade.FadeOutOverTime(false, ()=>SceneManager.LoadScene(1));
        };
    }


    public void OnEndAnimation()
    {
        cameraFade.FadeOutOverTime(false, ()=>SceneManager.LoadScene(1));
    }
    
}