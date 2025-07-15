using System.Runtime.CompilerServices;
using UnityEngine;

public class LensParallaxController : MonoBehaviour
{
    [SerializeField] private Camera primaryCamera;
    [SerializeField] private Transform[] controlledLayers;
    [SerializeField] private float maxCameraSize = 7.5f;

    private float initialCameraSize;
    private Vector3 initialLayerScale;

    private void Start()
    {
        initialCameraSize = primaryCamera.orthographicSize;
        initialLayerScale = controlledLayers[0].localScale;
    }

    private void Update()
    {
        // Calculate the parallax effect based on camera size
        float parallaxFactor = primaryCamera.orthographicSize / initialCameraSize;

        // Apply the parallax effect to the controlled layers
        for (int i = 0; i < controlledLayers.Length; i++)
        {
            if (controlledLayers[i] != null)
            {
                Vector3 newScale = Vector3.Lerp(
                    initialLayerScale,
                    Vector3.one,
                    (primaryCamera.orthographicSize - initialCameraSize) / (maxCameraSize - initialCameraSize));
                controlledLayers[i].localScale = newScale;
                //controlledLayers[i].localScale = initialLayerScale * parallaxFactor;
            }
        }
    }
}
