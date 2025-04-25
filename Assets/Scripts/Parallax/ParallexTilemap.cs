using UnityEngine;
using UnityEngine.Tilemaps;

public class ParallaxTilemap : MonoBehaviour
{
    private Vector2 startPos;      // Initial position of the tilemap
    private float length;          // Length of the tilemap in the X-axis
    private Camera cam;            // Reference to the main camera

    [Tooltip("0 = move with camera, 1 = no movement")]
    public float ParallaxAmountX;  // Controls parallax movement on the X-axis
    public float ParallaxAmountY;  // Controls parallax movement on the Y-axis
    public bool loop = true;

    private TilemapRenderer tilemapRenderer;  // Reference to the TilemapRenderer
    private Bounds tilemapBounds;             // Bounds of the tilemap
    public Vector2 camStartPos;               // Initial position of the camera

    void Start()
    {
        cam = Camera.main;
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapBounds = tilemapRenderer.localBounds;

        startPos = transform.position;                      // Store the initial position of the tilemap
        //camStartPos = cam.transform.position;              // Store the initial position of the camera
        length = tilemapBounds.size.x;                      // Get the length of the tilemap
    }

    void FixedUpdate()
    {
        // Calculate the distance the camera has moved since the start position
        float distanceX = (cam.transform.position.x - camStartPos.x) * ParallaxAmountX;
        float distanceY = (cam.transform.position.y - camStartPos.y) * ParallaxAmountY;

        // Apply the parallax effect on both X and Y axes, relative to the starting position
        transform.position = new Vector2(startPos.x + distanceX, startPos.y + distanceY);

        // Infinite scrolling logic for the X-axis
        float movementX = (cam.transform.position.x - camStartPos.x) * (1 - ParallaxAmountX);

        if (loop)
        {
            if (movementX > startPos.x + length)
            {
                startPos.x += length;
            }
            else if (movementX < startPos.x - length)
            {
                startPos.x -= length;
            }
        }
    }
}
