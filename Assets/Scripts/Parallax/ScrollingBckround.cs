using System.Diagnostics;
using UnityEngine;

public class ScrollingBckround : MonoBehaviour
{
    [Range(-15f, 15f)]
    public float scrollSpeed = 0.5f;

    private float offset;
    private Material mat;

    void Start()
    {
        // Cache the material on this object's Renderer
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Increment offset over time
        offset += (Time.deltaTime * scrollSpeed) / 10f;

        // Apply the offset to the _MainTex property of the material
        mat.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
