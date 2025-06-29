using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseController : MonoBehaviour
{
    [SerializeField] private Vector2 limits = new Vector2(0, 5);
    [SerializeField] private NoiseSO noiseSO;

    private int currentOffset = 1;
    private bool reverse = false;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();    
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
                if (currentOffset <= 0)
                {
                    reverse = false;
                }
            }
            else
            {
                currentOffset++;
                if (currentOffset >= 1000)
                {
                    reverse = true;
                }
            }
        }
    }

    private void DrawWave()
    {
        // Number of points in the saine wave
        lineRenderer.positionCount = noiseSO.resolution;

        int smt = noiseSO.anchor;
        //float current_y = Mathf.Lerp(0, 5, currentOffset / 1000f) - 2.5f;
        for (int current = 0; current < noiseSO.resolution; current++)
        {
            float x = Mathf.Lerp(limits.x, limits.y, current / (float)(noiseSO.resolution - 1)); // Normalized x position
            // Calculate the y position using a sine wave formula
            float time = Time.time;
            float y = noiseSO.amplitude * Mathf.Sin(2 * Mathf.PI * noiseSO.frequency * x + (time * noiseSO.speed));
            y += noiseSO.amplitude2 * Mathf.Sin(2 * Mathf.PI * noiseSO.frequency2 * x + (time * noiseSO.speed));
            //y += current_y;

            // Set the position of the point in the line renderer
            lineRenderer.SetPosition(current, new Vector3(x, y, 0f));
        }

        Vector2 endx = lineRenderer.GetPosition(smt);
        for (int pre = 0; pre < smt; pre++)
        {
            float pos = Mathf.Lerp(0, 1, pre / (float)(smt - 1));
            float a = lineRenderer.GetPosition(pre).y * pos;

            float smt_x = Mathf.Lerp(limits.x, endx.x, pre / (float)(smt - 1));
            lineRenderer.SetPosition(pre, new Vector2(smt_x, a));
        }

        endx = lineRenderer.GetPosition(noiseSO.resolution - smt);
        for (int pre = noiseSO.resolution - smt - 1; pre < noiseSO.resolution; pre++)
        {
            float pos = 1 - Mathf.Lerp(0, 1, (pre - (noiseSO.resolution - smt - 1)) / (float)(smt - 1));
            float a = lineRenderer.GetPosition(pre).y * pos;

            float smt_x = Mathf.Lerp(endx.x, limits.y, (pre - (noiseSO.resolution - smt - 1)) / (float)(smt - 1));
            lineRenderer.SetPosition(pre, new Vector2(smt_x, a));
        }
    }
}
