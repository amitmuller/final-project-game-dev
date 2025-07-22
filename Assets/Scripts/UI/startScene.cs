using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UI;

public class StartSceneController : MonoBehaviour
{
    [Header("Opening Screen (first frame)")]
    [Tooltip("Shown until the player presses Play")]
    public GameObject openingScreen;

    [SerializeField]
    private GameObject animation;
    
    [SerializeField]
    private CameraFade cameraFade;
    
    [Header("Audio Sources")]
    [Tooltip("Looping nature ambience")]
    [SerializeField] private AudioSource ambienceSource;
    

    void Awake()
    {
        // show only opening screen
        openingScreen.SetActive(true);
        if (ambienceSource != null)
        {
            ambienceSource.loop = true;
            ambienceSource.Play();
        }

    }

    /// <summary>
    /// Bound to your “Play” action. First press hides the opening screen
    /// and launches the rest of the frames.
    /// </summary>
    public void OnPressPlay(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        openingScreen.SetActive(false);
        animation.SetActive(true);
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