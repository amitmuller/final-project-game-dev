using Characters.Player;
using DG.Tweening;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform autoMoveDestination;
    [SerializeField] private float autoMoveDuration = 2f;

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
    }
}
