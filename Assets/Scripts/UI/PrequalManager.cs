using System.Collections;
using System.Collections.Generic;
using Characters.Player;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;



    public class PrequalManager : MonoBehaviour
    {
        [Header("GameObjects")]
        [SerializeField] private GameObject Guard;
        [SerializeField] private GameObject motherDino;
        [SerializeField] private GameObject player;
        
        [Header("Effects")]
        [SerializeField] private CameraFade cameraFade;
        
        [Header("Timing")]
        [SerializeField] private float openEyestime = 2f;
        [SerializeField] private float sleeptime2 = 2f;
    
        
        private characterAnimation _animation;
        private characterMovement  _characterMovement;
        private bool afterAnimation = false;
        private bool sequenceStarted = false;

        private void Start()
        {
            
            _animation = player.GetComponent<characterAnimation>();
            _characterMovement = player.GetComponent<characterMovement>();
            
            _characterMovement.SetCanMove(false);
            _animation.TransitionTo(PlayerAnimState.Sleep);
            StartCoroutine(OpenEyes());

        }


        IEnumerator OpenEyes()
        {
            yield return new WaitForSeconds(openEyestime);  
            _animation.TransitionTo(PlayerAnimState.SleepyEyes, entry => onCloseEyes());
        }


        void onCloseEyes()
        {
            cameraFade.FadeOutOverTime();
            Guard.SetActive(false);
            motherDino.SetActive(false);
            cameraFade.FadeOutOverTime(true);
            StartCoroutine(startGame());
        }

        IEnumerator startGame()
        {
            yield return new WaitForSeconds(sleeptime2);
            _animation.TransitionTo(PlayerAnimState.Awake);
            _animation.startBlink();
            _characterMovement.SetCanMove(true);
        }
        
        void Update()
        {
            // F1: normal fade out
            if (Input.GetKeyDown(KeyCode.P))
            {
                _animation.TransitionTo(PlayerAnimState.Awake);
                _animation.startBlink();
                _characterMovement.SetCanMove(true);
            }

           
        }
        


    }

