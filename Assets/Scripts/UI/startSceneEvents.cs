using System.Collections;
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
    
    
    public void OnTrain()
    {
        if (trainSource != null)
        {
            trainSource.loop = false;
            trainSource.Play();
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

    public void OnEndAnimation()
    {
        cameraFade.FadeOutOverTime(false, ()=>SceneManager.LoadScene(1));
    }

    void Update()
    {
        // cheat: skip to end
        if (Keyboard.current.sKey.wasPressedThisFrame)
            SceneManager.LoadScene(1);
    }
}