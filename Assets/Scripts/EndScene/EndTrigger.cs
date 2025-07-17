using Characters.Player;
using DG.Tweening;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform autoMoveDestination;
    [SerializeField] private float autoMoveDuration = 2f;

    [SerializeField] private CinemachineBrain mainCameraBrain;
    [SerializeField] private GameObject endCamera;

    [SerializeField] private float finalSceneDelay = 2f;

    private characterAnimation charAnim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.GetComponent<characterMovement>().SetCanMove(false);
            charAnim = player.GetComponent<characterAnimation>();
            charAnim.TransitionTo(PlayerAnimState.Walk);
            Vector3 destination = player.transform.position;
            destination.x = autoMoveDestination.position.x;
            player.transform.DOMove(destination, autoMoveDuration)
                .onComplete = OnPlayerReachedDestination;
        }
    }

    private void OnPlayerReachedDestination()
    {
        charAnim.TransitionTo(PlayerAnimState.Idle);

        endCamera.SetActive(true);
        var currentCamera = mainCameraBrain.ActiveVirtualCamera as CinemachineCamera;
        currentCamera.gameObject.SetActive(false);

        StartCoroutine(FinalSceneStarter());
    }

    private IEnumerator FinalSceneStarter()
    {
        Debug.Log($"Waiting for {finalSceneDelay}");
        yield return new WaitForSeconds(finalSceneDelay);
        SceneManager.LoadScene(2);
    }
}
