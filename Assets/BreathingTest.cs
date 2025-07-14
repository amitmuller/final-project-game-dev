using System.Collections;
using UnityEngine;

public class BreathingTest : MonoBehaviour
{
    [Header("Glow Effect Settings")]
    [SerializeField] private Color glowColorA = Color.white;
    [SerializeField] private Color glowColorB = Color.white;
    [SerializeField] private float glowColorTransitionSpeed = 1.0f;

    [Header("Breathing Effect Settings")]
    [SerializeField] private float maxIntensity = 6.0f;
    [SerializeField] private float minIntensity = 2.0f;
    [SerializeField] private float breathingSpeed = 1.0f;

    [Header("Shader Property Settings")]
    [Tooltip("The shader property name for the glow color")]
    [SerializeField] private string glowColorProperty = "_GlowColor"; // The name of the shader property for the glow color

    private Material material;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        // Glow color transition effect
        Color glow = Color.Lerp(
            glowColorA, glowColorB, Mathf.PingPong(Time.time * glowColorTransitionSpeed, 1.0f));
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(Time.time * breathingSpeed, 1.0f));
        glow.a = intensity;
        //Color glow = Color.Lerp(
        //    currentGlowColor, targetGlowColor, Time.deltaTime * glowColorTransitionSpeed);

        // Breathing effect
        //glow = Color.Lerp(
        //    new Color(glow.r, glow.g, glow.b, minIntensity),
        //    new Color(glow.r, glow.g, glow.b, maxIntensity),
        //    Mathf.PingPong(Time.time * breathingSpeed, 1.0f)
        //);

        // Apply the glow color to the material
        material.SetColor(glowColorProperty, glow);
    }
}
