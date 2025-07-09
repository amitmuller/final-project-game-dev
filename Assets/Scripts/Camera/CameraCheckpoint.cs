using Unity.Cinemachine;
using UnityEngine;

public class CameraCheckpoint : MonoBehaviour
{
    [SerializeField] private CinemachineCamera outgoingCamera;
    [SerializeField] private CinemachineCamera incomingCamera;

    private CinemachineCamera activeCamera;
    private CinemachineCamera inactiveCamera;

    private void Awake()
    {
        if ((null == outgoingCamera) || (null == incomingCamera))
        {
            Debug.LogError($"Cinemachine cameras are not assigned in the CameraCheckpoint {gameObject.name}");
            return;
        }

        activeCamera = outgoingCamera;
        inactiveCamera = incomingCamera;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only the player is allowed to trigger the checkpoint
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        activeCamera.gameObject.SetActive(false);
        inactiveCamera.gameObject.SetActive(true);

        // Set the follow of the new active camera to the player
        inactiveCamera.Follow = collision.transform;

        // Swap the active and inactive cameras
        CinemachineCamera temp = activeCamera;
        activeCamera = inactiveCamera;
        inactiveCamera = temp;
    }
}
