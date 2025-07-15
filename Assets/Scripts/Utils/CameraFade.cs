using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    [SerializeField] private Image fade;
    [SerializeField] private float duration = 3f;
    private readonly Color _startColor = new Color(0, 0, 0, 0);
    private readonly Color _endColor   = new Color(0, 0, 0, 1);

    private Coroutine _fadeCoroutine;

    /// <summary>
    /// Fade duration.
    /// </summary>
    public float Duration => duration;

    private void Awake()
    {
        if (fade != null)
        {
            fade.gameObject.SetActive(true);
            fade.color = _startColor;
        }
    }

    /// <summary>
    /// Fades out (transparent→black) then in (black→transparent),
    /// invoking onBlack when fully black, and onComplete when the full cycle finishes.
    /// </summary>
    /// <param name="onBlack">Called once the fade reaches full black.</param>
    /// <param name="onComplete">Called after fading back to transparent.</param>
    public void FadeOutAndIn(Action onBlack = null, Action onComplete = null)
    {
        if (fade == null) return;
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeSequence(onBlack, onComplete));
    }

    private IEnumerator FadeSequence(Action onBlack, Action onComplete)
    {
        // 1) transparent → black, then trigger onBlack
        yield return StartCoroutine(LerpColor(_startColor, _endColor, onBlack));
        // 2) black → transparent
        yield return StartCoroutine(LerpColor(_endColor, _startColor, null));

        // full cycle complete
        onComplete?.Invoke();
        _fadeCoroutine = null;
    }

    /// <summary>
    /// Fades between start and end over duration, invoking onReach once at the end.
    /// </summary>
    /// <param name="from">Starting color.</param>
    /// <param name="to">Target color.</param>
    /// <param name="onReach">Called once fade completes to 'to'.</param>
    private IEnumerator LerpColor(Color from, Color to, Action onReach)
    {
        float elapsed = 0f;
        fade.color = from;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = Mathf.SmoothStep(0f, 1f, t);
            fade.color = Color.Lerp(from, to, eased);
            yield return null;
        }
        fade.color = to;
        onReach?.Invoke();
    }

    /// <summary>
    /// Fades to (reverse==false) or from (reverse==true) black over duration.
    /// </summary>
    /// <param name="reverse">If true, fade black→transparent; otherwise transparent→black.</param>
    /// <param name="onReach">Called once fade completes.</param>
    public void FadeOutOverTime(bool reverse = false, Action onReach = null)
    {
        if (fade == null) return;
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        Color from = reverse ? _endColor : _startColor;
        Color to   = reverse ? _startColor : _endColor;
        _fadeCoroutine = StartCoroutine(LerpColor(from, to, onReach));
    }

    private void Update()
    {
        // testing shortcuts
        if (Input.GetKeyDown(KeyCode.F1))
        {
            FadeOutOverTime(false, () => Debug.Log("Reached black via F1"));
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            FadeOutOverTime(true, () => Debug.Log("Faded back via F2"));
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            FadeOutAndIn(
                onBlack: () => Debug.Log("Screen is fully black now!"),
                onComplete: () => Debug.Log("Fade out and in complete."));
        }
    }
}