using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable_objects;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // for ThrowableObject

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Each Cart’s data")]
    public List<CartData> carts = new List<CartData>();

    // Spare templates (_Spare) per cart
    private List<List<GameObject>> _spareThrowableRoots = new List<List<GameObject>>();
    
    [SerializeField] private CameraFade _cameraFade;
    [SerializeField] private float checkpointDelay = 1f;
    [SerializeField] private MMF_Player feedbackCheckpoint;
    private int currentCart = 0;
    
    [Header("Each Cart’s data")]
    [SerializeField] public GameObject PauseMenu;
    [SerializeField] public float timeToOpenScene;
    private bool inPause = false;
    private Coroutine openSceneCoroutine;
    public static event Action OnPlayerDead;
    public static event Action OnPlayerRevived;

    private void Awake()
    {
        // Singleton
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Create one spare copy for each throwable in each cart
        foreach (var cart in carts)
        {
            var spareList = new List<GameObject>();
            foreach (var comp in cart.throwables)
            {
                // Clone full hierarchy under same parent
                Transform parent = comp.transform.parent;
                GameObject spare = Instantiate(
                    comp.gameObject,
                    comp.transform.position,
                    comp.transform.rotation,
                    parent
                );
                spare.name = comp.gameObject.name + "_Spare";
                spare.SetActive(false);
                spare.hideFlags = HideFlags.HideInHierarchy;
                spareList.Add(spare);
            }
            _spareThrowableRoots.Add(spareList);
        }

        _cameraFade.FadeOutOverTime(true);
    }

    public void PlayerEnteredCart(int cartIndex)
    {
        currentCart = cartIndex;
        if (cartIndex < 0 || cartIndex >= carts.Count)
        {
            Debug.LogError($"PlayerEnteredCart: invalid index {cartIndex}.");
            return;
        }

        var cart = carts[cartIndex];
        if (!cart.hasActivated)
        {
            ActivateEnemiesInCart(cart);
            cart.hasActivated = true;
            Debug.Log($"[GameManager] Activated enemies for {cart.cartName}.");
        }
    }

    public void PlayerLeftCart(int cartIndex)
    {
        if (cartIndex < 0 || cartIndex >= carts.Count) return;
        var cart = carts[cartIndex];
        if (cart.hasActivated)
        {
            DisableEnemiesInCart(cart);
            cart.hasActivated = false;
            Debug.Log($"[GameManager] Deactivated enemies for {cart.cartName}.");
        }
    }

    public void checkpoint(Transform player)
    {
        if (player == null) return;
        // feedbackCheckpoint.PlayFeedbacks();
        OnPlayerDead?.Invoke();
        StartCoroutine(CheckpointRoutine(player));
    }

    private IEnumerator CheckpointRoutine(Transform player)
    {
        yield return new WaitForSeconds(checkpointDelay);
        OnPlayerRevived?.Invoke();
        _cameraFade.FadeOutOverTime(true);
        ResetEnemiesInCart();
        ResetThrowables();
        player.position = carts[currentCart].checkpointPosition;
    }
    

    private void ActivateEnemiesInCart(CartData cart)
    {
        foreach (var enemy in cart.enemies)
            if (enemy != null)
                enemy.SetActive(true);
    }

    private void DisableEnemiesInCart(CartData cart)
    {
        foreach (var enemy in cart.enemies)
        {
            if (enemy == null) continue;
            var ctrl = enemy.GetComponent<EnemyAIController>();
            if (ctrl != null) ctrl.ResetEnemy();
            enemy.SetActive(false);
        }
    }

    private void ResetEnemiesInCart()
    {
        var cart = carts[currentCart];
        foreach (var enemy in cart.enemies)
        {
            if (enemy == null) continue;
            var ctrl = enemy.GetComponent<EnemyAIController>();
            if (ctrl != null) ctrl.ResetEnemy();
        }
    }

    private void ResetThrowables()
    {
        var cart = carts[currentCart];

        // 1) Destroy all fragments
        foreach (var frag in FindObjectsOfType<FragmentBehavior>())
            Destroy(frag.gameObject);

        // 2) Destroy any active throwables
        foreach (var comp in cart.throwables)
            if (comp != null)
                Destroy(comp.gameObject);
        cart.throwables.Clear();

        // 3) Activate spares
        var spares = _spareThrowableRoots[currentCart];
        foreach (var spare in spares)
        {
            spare.SetActive(true);
            var newComp = spare.GetComponent<ThrowableObject>();
            if (newComp != null)
                cart.throwables.Add(newComp);
            else
                Debug.LogWarning($"[{spare.name}] no ThrowableObject found!");
        }

        // 4) Rebuild spares for next reset
        var newSpareList = new List<GameObject>();
        foreach (var comp in cart.throwables)
        {
            Transform parent = comp.transform.parent;
            GameObject spare = Instantiate(
                comp.gameObject,
                comp.transform.position,
                comp.transform.rotation,
                parent
            );
            spare.name = comp.gameObject.name + "_Spare";
            spare.SetActive(false);
            spare.hideFlags = HideFlags.HideInHierarchy;
            newSpareList.Add(spare);
        }
        _spareThrowableRoots[currentCart] = newSpareList;
    }

    public void onPause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!inPause)
        {
            enterPause();
        }
        else
        {
            exitPause();
        }
    }

    public void enterPause()
    {
        inPause = true;
        PauseMenu.SetActive(true);
        MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0f, 0f, true, 50f, true);
        openSceneCoroutine = StartCoroutine(goToOpenScene());
    }
    public void exitPause()
    {
        PauseMenu.SetActive(false);
        inPause = false;
        if (openSceneCoroutine != null)
        {
            StopCoroutine(openSceneCoroutine);
        }
        MMTimeScaleEvent.Unfreeze();
    }

    private IEnumerator goToOpenScene()
    {

        yield return new WaitForSecondsRealtime(timeToOpenScene);
        openSceneCoroutine = null;
        onOpenScene();
    }

    public void onOpenScene()
    {
        exitPause();
        SceneManager.LoadScene(0);
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public bool getInPause => inPause;
}
