using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [Tooltip("Time the light stays off during flicker, in seconds")]
    [SerializeField] private float turnoffTime = 0.25f;

    [Tooltip("Time between flickers, in seconds")]
    [SerializeField] private float flickerInterval = 4f;

    [Tooltip("Number of continuous flickers before the interval is engaged")]
    [Min(1)]
    [SerializeField] private int flickersCount = 2;

    private Light2D _light;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
    }

    private void Start()
    {
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            for (int i = 0; i < flickersCount; i++)
            {
                _light.enabled = false;
                yield return new WaitForSeconds(turnoffTime);
                _light.enabled = true;
                yield return new WaitForSeconds(turnoffTime);
            }
            yield return new WaitForSeconds(flickerInterval);
        }
    }

}
