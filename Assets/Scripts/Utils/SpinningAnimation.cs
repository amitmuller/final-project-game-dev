using DG.Tweening;
using UnityEngine;

public class SpinningAnimation : MonoBehaviour
{
    [SerializeField] private float fullRotationDuration = 5f;

    private Tweener rotationAnim;

    private void Start()
    {
        rotationAnim = transform.DORotate(new Vector3(0, 0, 360), fullRotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    private void OnValidate()
    {
        rotationAnim?.ChangeValues(Vector3.zero,
            new Vector3(0, 0, 360),
            fullRotationDuration);
    }
}
