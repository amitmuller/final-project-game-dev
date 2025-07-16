using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraCheckpoint : MonoBehaviour
{
    public event EventHandler<OnPlayerPassedCheckpointEventArgs> OnPlayerPassedCheckpoint;
    public class OnPlayerPassedCheckpointEventArgs : EventArgs
    {
        public bool isGoingOutdoors;
    }

    [Tooltip("Whether the checkpoints leads to an outdoor environment")]
    [SerializeField] private bool isIncomingOutdoor = false;
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

        // If isIncomingOutdoor=true, then this means the player is going outdoors
        if (collision.attachedRigidbody.linearVelocityX > 0)
        {
            incomingCamera.gameObject.SetActive(true);
            outgoingCamera.gameObject.SetActive(false);

            incomingCamera.Follow = collision.transform;

            OnPlayerPassedCheckpoint?.Invoke(this, new OnPlayerPassedCheckpointEventArgs
            {
                isGoingOutdoors = isIncomingOutdoor
            });
        }
        // If isIncomingOutdoor=false, then this means the player is going indoors
        else if (collision.attachedRigidbody.linearVelocityX < 0)
        {
            incomingCamera.gameObject.SetActive(false);
            outgoingCamera.gameObject.SetActive(true);

            outgoingCamera.Follow = collision.transform;

            OnPlayerPassedCheckpoint?.Invoke(this, new OnPlayerPassedCheckpointEventArgs
            {
                isGoingOutdoors = !isIncomingOutdoor
            });
        }
    }
}
