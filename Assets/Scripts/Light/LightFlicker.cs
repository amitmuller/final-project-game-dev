using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [Tooltip("Maximum delay before the flicker starts, in seconds")]
    [SerializeField] private float maxStartDelay = 4.0f;
    [Tooltip("Time between flickers, in seconds")]
    [SerializeField] private float flickerInterval = 4f;
    [SerializeField] private float minLightIntensity = 0f;
    [SerializeField] private float maxLightIntensity = 1f;
    [SerializeField] private float flickerSpeed = 0.1f;
    [SerializeField] private float flickerDuration = 0.5f;

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
        // Wait for a random time before starting the flicker
        yield return new WaitForSeconds(Random.Range(0f, maxStartDelay));

        while (true)
        {
            float elapsedTime = 0f;

            while (elapsedTime < flickerDuration)
            {
                _light.intensity = Mathf.Lerp(
                    minLightIntensity, 
                    maxLightIntensity, 
                    Mathf.PingPong(Time.time * flickerSpeed, 1));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _light.intensity = maxLightIntensity; // Ensure light is at max intensity before flickering off

            yield return new WaitForSeconds(flickerInterval);
        }
    }

}
