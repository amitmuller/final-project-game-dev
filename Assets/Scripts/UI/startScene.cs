using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;



    public class StartSceneController : MonoBehaviour
    {
        [Header("Frame Sequence")]
        [Tooltip("List of frames to show sequentially with durations.")]
        [SerializeField] private SequenceFrame openingFrame;
        [SerializeField] private List<SequenceFrame> frames = new List<SequenceFrame>();
        
    
        private bool afterAnimation = false;
        private bool sequenceStarted = false;

        private void Awake()
        {
            // Ensure all frames are hidden initially
            foreach (var frame in frames)
            {
                if (frame.frameObject != null)
                    frame.frameObject.SetActive(false);
            }
            openingFrame.frameObject.SetActive(true);
        }
        

        /// <summary>
        /// Bound to your "Play" input action in the Input System.
        /// </summary>
        public void OnPressPlay(InputAction.CallbackContext context)
        {
            
            if (!context.performed || sequenceStarted)
                return;
            openingFrame.frameObject.SetActive(false);

            StartCoroutine(PlaySequence());
        }

        /// <summary>
        /// Plays frames one after another, each for its specified duration.
        /// </summary>
        private IEnumerator PlaySequence()
        {
            int totalFrames = frames.Count;
            for (int i = 0; i < totalFrames; i++)
            {
                frames[i].frameObject.SetActive(true);
                yield return new WaitForSecondsRealtime(frames[i].displayTime);
                if (i == totalFrames-1)
                {
                    OnEndScene();
                }
                frames[i].frameObject.SetActive(false);
                
            }
        }
        
        void Update() {
            if (Input.GetKeyDown(KeyCode.S))
                OnEndScene();
        }

        /// <summary>
        /// Loads the game scene (index 1).
        /// </summary>
        public void OnEndScene()
        {
            SceneManager.LoadScene(1);
        }
    }

