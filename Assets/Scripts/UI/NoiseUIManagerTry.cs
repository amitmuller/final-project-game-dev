using System.Collections;
using UnityEngine;
using Radishmouse;   // ← your namespace for UILineRenderer

[RequireComponent(typeof(RectTransform), typeof(UILineRenderer))]
public class NoiseUIController : MonoBehaviour
{
    [SerializeField] private Vector2 limits = new Vector2(0, 5);
    [SerializeField] private NoiseSO noiseSO;

    private bool reverse = false;
    private int currentOffset = 1;

    private UILineRenderer uiLine;
    private RectTransform rectTransform;

    private void Awake()
    {
        uiLine = GetComponent<UILineRenderer>();
        rectTransform = GetComponent<RectTransform>();

        // Make sure your line is drawn relative to the bottom‐left of the rect:
        uiLine.center = false;
    }

    private void Start()
    {
        StartCoroutine(OffsetUpdater());
    }

    private void Update()
    {
        DrawWave();
    }

    private IEnumerator OffsetUpdater()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.005f);
            if (reverse)
            {
                currentOffset--;
                if (currentOffset <= 0) reverse = false;
            }
            else
            {
                currentOffset++;
                if (currentOffset >= 1000) reverse = true;
            }
        }
    }

    private void DrawWave()
    {
        int res = noiseSO.resolution;
        float now = Time.time;

        // 1) grid count
        uiLine.points = new Vector2[res];

        // 2) clamp maxAmp
        float maxAmp = Mathf.Abs(noiseSO.amplitude) + Mathf.Abs(noiseSO.amplitude2);
        if (maxAmp <= Mathf.Epsilon) maxAmp = 1f;

        float W = rectTransform.rect.width;
        float H = rectTransform.rect.height;
        Vector2[] pts = new Vector2[res];

        for (int i = 0; i < res; i++)
        {
            float t      = i / (float)(res - 1);
            float worldX = Mathf.Lerp(limits.x, limits.y, t);

            // raw wave
            float rawY =
                noiseSO.amplitude  * Mathf.Sin(2 * Mathf.PI * noiseSO.frequency  * worldX + now * noiseSO.speed)
                + noiseSO.amplitude2 * Mathf.Sin(2 * Mathf.PI * noiseSO.frequency2 * worldX + now * noiseSO.speed);

            float xUI = t * W;
            float yUI = (rawY / maxAmp) * (H * 0.5f) + (H * 0.5f);
            if (!float.IsFinite(yUI)) yUI = H * 0.5f;

            pts[i] = new Vector2(xUI, yUI);
        }

        // 3) fade-in/fade-out anchors (same as before), but only if smt < res
        int smt = Mathf.Clamp(noiseSO.anchor, 1, res - 1);
        Vector2 startFadeTarget = pts[smt];
        Vector2 endFadeStart    = pts[res - smt - 1];
        for (int i = 0; i < smt; i++)
        {
            float p = i / (float)(smt - 1);
            pts[i] = Vector2.Lerp(Vector2.zero, startFadeTarget, p);
        }
        for (int i = res - smt; i < res; i++)
        {
            float p = (i - (res - smt)) / (float)(smt - 1);
            pts[i] = Vector2.Lerp(endFadeStart, new Vector2(W, 0), p);
        }

        // 4) finally hand off to the UI line
        uiLine.points = pts;
        uiLine.SetVerticesDirty();
    }

}
