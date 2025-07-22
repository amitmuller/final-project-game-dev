using DG.Tweening;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class LensParallaxController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera primaryCamera;
    [SerializeField] private float maxCameraSize = 7.5f;

    [Header("Parallax Layer Settings")]
    [SerializeField] private Transform[] controlledLayers;
    [SerializeField] private SpriteRenderer glowLayer;
    [SerializeField] private float maxIntensity = 6.0f;
    [SerializeField] private float minIntensity = 4.0f;

    private float initialCameraSize;
    private Vector3 initialLayerScale;
    private Material glowMaterial;
    private Color baseColor;
    private bool isManualScale = false;

    private void Start()
    {
        initialCameraSize = primaryCamera.orthographicSize;
        initialLayerScale = controlledLayers[0].localScale;
        glowMaterial = glowLayer.material;
        baseColor = glowMaterial.GetColor("_GlowColor");
    }

    private void Update()
    {
        if (isManualScale)
        {
            return;
        }

        // Calculate the parallax effect based on camera size
        float currentOffset = (primaryCamera.orthographicSize - initialCameraSize) / (maxCameraSize - initialCameraSize);

        // Apply the parallax effect to the controlled layers
        for (int i = 0; i < controlledLayers.Length; i++)
        {
            if (controlledLayers[i] != null)
            {
                Vector3 newScale = Vector3.Lerp(
                    initialLayerScale,
                    Vector3.one,
                    currentOffset);
                controlledLayers[i].localScale = newScale;
            }
        }

        float newIntensity = Mathf.Lerp(maxIntensity, minIntensity, currentOffset);
        glowMaterial.SetColor("_GlowColor", baseColor * newIntensity);
    }

    public void SetParallaxScale(float scale)
    {
        isManualScale = true;
        for (int i = 0; i < controlledLayers.Length; i++)
        {
            if (controlledLayers[i] != null)
            {
                controlledLayers[i].DOScale(scale, 0.5f);
            }
        }
    }
}
