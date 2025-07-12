using MoreMountains.Feedbacks;
using UnityEngine;

public class CameraShakeFeedback : MonoBehaviour
{
    private MMF_Player MMF_Player;

    private void Awake()
    {
        MMF_Player = GetComponent<MMF_Player>();
    }

    private void Start()
    {
        MMF_Player.PlayFeedbacks();
    }
}
