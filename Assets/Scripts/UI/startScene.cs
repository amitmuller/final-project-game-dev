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

    [Header("Rest of the Sequence")]
    [Tooltip("Drop in your SequenceFrame components here, in order")]
    public SequenceFrame[] frames;

    private bool _sequenceStarted = false;

    void Awake()
    {
        // show only opening screen
        openingScreen.SetActive(true);
        // hide all sequence frames
        foreach (var f in frames)
            f.gameObject.SetActive(false);
    }

    /// <summary>
    /// Bound to your “Play” action. First press hides the opening screen
    /// and launches the rest of the frames.
    /// </summary>
    public void OnPressPlay(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || _sequenceStarted) return;
        _sequenceStarted = true;

        openingScreen.SetActive(false);
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // loop through each frame
        foreach (var frame in frames)
        {
            // show this frame’s GameObject
            frame.gameObject.SetActive(true);
            // invoke your custom logic
            frame.PlayFrame();

            // wait its duration
            yield return new WaitForSecondsRealtime(frame.displayTime);

            // hide it unless it’s the last one
            if (frame != frames[frames.Length - 1])
                frame.gameObject.SetActive(false);
        }

        // done → load next scene
        SceneManager.LoadScene(1);
    }

    void Update()
    {
        // cheat: skip to end
        if (Keyboard.current.sKey.wasPressedThisFrame)
            SceneManager.LoadScene(1);
    }
}