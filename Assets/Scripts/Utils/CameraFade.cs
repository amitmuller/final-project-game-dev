using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    [SerializeField] private Image fade;
    [SerializeField] private float duration = 3f;
    private readonly Color _startColor = new Color(0, 0, 0, 0);
    private readonly Color _endColor = new Color(0, 0, 0, 1);

    private Coroutine _fadeOutCoroutine;

    public float Duration => duration;

    private void Awake()
    {
        fade.gameObject.SetActive(true);
    }

    public void FadeOutOverTime(bool reverse = false)
    {
        if (!fade) return;
        if (_fadeOutCoroutine != null)
            StopCoroutine(_fadeOutCoroutine);

        _fadeOutCoroutine = StartCoroutine(LerpColor(reverse));
    }
    
    /// <summary>
    /// New: fade transparent → black → transparent in one call
    /// </summary>
    public void FadeOutAndIn()
    {
        if (!fade) return;
        if (_fadeOutCoroutine != null)
            StopCoroutine(_fadeOutCoroutine);

        _fadeOutCoroutine = StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // 1) transparent → black
        yield return StartCoroutine(LerpColor(false));

        // 2) black → transparent (immediate, no extra delay)
        float tTime = 0f;
        while (tTime < duration)
        {
            tTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(tTime / duration);
            float eased = Mathf.SmoothStep(0f, 1f, t);
            fade.color = Color.Lerp(_endColor, _startColor, eased);
            yield return null;
        }
        fade.color = _startColor;

        _fadeOutCoroutine = null;
    }

    private IEnumerator LerpColor(bool reverse)
    {
        // if (reverse) yield return new WaitForSecondsRealtime(2f);
        float time = 0f;

        Color fromColor = reverse ? _endColor : _startColor;
        Color toColor   = reverse ? _startColor : _endColor;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            float easedT = Mathf.SmoothStep(0f, 1f, t); // Smooth easing
            fade.color = Color.Lerp(fromColor, toColor, easedT);
            yield return null;
        }

        fade.color = toColor;
        _fadeOutCoroutine = null;
    }
    
    void Update()
    {
        // F1: normal fade out
        if (Input.GetKeyDown(KeyCode.F1))
        {
            FadeOutOverTime();
            Debug.Log("Cheat: FadeOutOverTime() triggered");
        }

        // F2: fade back in (reverse)
        if (Input.GetKeyDown(KeyCode.F2))
        {
            FadeOutOverTime(true);
            Debug.Log("Cheat: FadeOutOverTime(reverse) triggered");
        }

        // F3: fade out then back in
        if (Input.GetKeyDown(KeyCode.F3))
        {
            FadeOutAndIn();
            Debug.Log("Cheat: FadeOutAndIn() triggered");
        }
    }
}