using System;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI
{
    public class startScene:MonoBehaviour
    {
        
        [SerializeField] private GameObject startCaption;
        [SerializeField] private MMF_Player feedback;
        private Animator _animator;
        private bool afterAnimation = false; 

        private void Awake()
        {
            startCaption.SetActive(false);
            _animator = GetComponent<Animator>();
        }

        public void onEndStartAnimation()
        {
            startCaption.SetActive(true);
            afterAnimation = true;
        }
        
        public void OnPressPlay(InputAction.CallbackContext context)
        {
            if (afterAnimation)
            {
                startCaption.SetActive(false);
                _animator.SetTrigger("start");
            }
        }
        
        public void OnEndScene()
        {
            SceneManager.LoadScene(1);
        }
    }
    
    
}