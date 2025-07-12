using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject pauseMenuCanvas;
    [SerializeField] GameObject firstSelectedButton;
    
    bool      isPaused = false;
    
    
    // Hook these up to your UI Buttons in the Inspector:
    public void OnResume()       =>  GameManager.Instance.exitPause();
    
    public void OnQuitToMenu() =>   SceneManager.LoadScene(0);
    public void OnRestart()     { /* open settings sub-panel */ }
}