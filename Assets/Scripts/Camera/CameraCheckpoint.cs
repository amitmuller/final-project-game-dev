using Unity.Cinemachine;
using UnityEngine;

public class CameraCheckpoint : MonoBehaviour
{
    [SerializeField] private CinemachineCamera outgoingCamera;
    [SerializeField] private CinemachineCamera incomingCamera;

    private void Awake()
    {
        if ((null == outgoingCamera) || (null == incomingCamera))
        {
            Debug.LogError($"Cinemachine cameras are not assigned in the CameraCheckpoint {gameObject.name}");
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Only the player is allowed to trigger the checkpoint
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (collision.attachedRigidbody.linearVelocityX > 0)
        {
            incomingCamera.gameObject.SetActive(true);
            outgoingCamera.gameObject.SetActive(false);

            incomingCamera.Follow = collision.transform;
        }
        else if (collision.attachedRigidbody.linearVelocityX < 0)
        {
            incomingCamera.gameObject.SetActive(false);
            outgoingCamera.gameObject.SetActive(true);

            outgoingCamera.Follow = collision.transform;
        }
    }
}
