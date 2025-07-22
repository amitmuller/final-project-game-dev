using DG.Tweening;
using UnityEngine;

public class ChandelierSway : MonoBehaviour
{
    [SerializeField] private float swayDuration = 2f;
    [SerializeField] private float swayAngle = 10f;

    private Tweener swayTween;
    private Vector3 startAngles;

    private void Awake()
    {
        startAngles = transform.eulerAngles; // Saving the initial rotation for resetting later
        transform.eulerAngles -= new Vector3(0, 0, swayAngle);
    }

    private void Start()
    {
        StartAnimation();
    }

    // Updating the values of the animation upon change in the inspector
    private void OnValidate()
    {
        swayTween?.ChangeValues(
            startAngles - new Vector3(0, 0, swayAngle), 
            startAngles + new Vector3(0, 0, swayAngle), 
            swayDuration);
    }

    private void StartAnimation()
    {
        swayTween = transform.DORotate(
            transform.eulerAngles + new Vector3(0, 0, 2 * swayAngle), swayDuration, RotateMode.Fast)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        swayTween?.Kill();
    }
}
