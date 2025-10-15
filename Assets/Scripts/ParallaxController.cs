using UnityEngine;

/// <summary>
/// Controls the movement of multiple background layers to create a Parallax scrolling effect
/// and automatically tiles them to create an infinitely long background.
/// </summary>
public class ParallaxController : MonoBehaviour
{
    // A structure to hold both the background Transform and its parallax factor, plus tiling info.
    [System.Serializable]
    public struct ParallaxLayer
    {
        [Tooltip("The Transform of the individual background tile.")]
        public Transform background;
        [Tooltip("How much slower or faster this layer moves relative to the camera.")]
        [Range(0f, 2f)]
        public float parallaxFactor;
        [Tooltip("The width of the background tile in Unity units (e.g., 20.48). This MUST be accurate for seamless tiling.")]
        public float tileWidth;
        // The parallaxGroup ID is kept for future grouping but not strictly necessary for simple tiling.
        public int parallaxGroup;
    }

    [Header("Setup")]
    [Tooltip("The Transform of the camera this script should follow (usually Main Camera).")]
    public Transform mainCamera;

    [Header("Layers")]
    [Tooltip("Drag all ORIGINAL background tiles and their respective COPIES here.")]
    public ParallaxLayer[] layers;

    private Vector3 lastCameraPosition;

    void Start()
    {
        // Safety check to ensure the camera is assigned.
        if (mainCamera == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                mainCamera = cam.transform;
            }
            else
            {
                Debug.LogError("ParallaxController requires a Main Camera. Please assign one.");
                enabled = false;
                return;
            }
        }

        // Store the camera's starting position for movement calculation.
        lastCameraPosition = mainCamera.position;
    }

    void LateUpdate()
    {
        // 1. Calculate how much the camera has moved since the last frame.
        Vector3 cameraDelta = mainCamera.position - lastCameraPosition;

        // 2. Iterate through all background layers (originals and copies) and apply the movement.
        foreach (var layer in layers)
        {
            if (layer.background == null) continue;

            // Apply Parallax Movement
            // --------------------------------------------------------------------------------

            // Calculate the proportional movement for this layer.
            // movement.x will be used for horizontal parallax
            Vector3 movement = cameraDelta * layer.parallaxFactor;

            // Apply the movement to the background layer's position (only modifying X and Y).
            layer.background.position += new Vector3(movement.x, movement.y, 0f);

            // Apply Tiling/Looping Logic (Horizontal only)
            // --------------------------------------------------------------------------------

            // Calculate half of the total tiling area needed for the loop to work (since we have 2 tiles).
            // The boundary is the camera position plus or minus one tile width.

            // If the layer moves slower than the camera, the layer's parallax factor will be < 1.0.
            // As the camera moves right, the layer moves left relative to the camera center.

            float distanceToCameraX = mainCamera.position.x - layer.background.position.x;

            // Check if the tile has moved fully out of view to the left (player moving right).
            // We check if the camera's position relative to the layer's start is greater than 
            // half the total loop distance (one tile width).
            if (distanceToCameraX > layer.tileWidth)
            {
                // Snap the tile forward by exactly one tile width.
                // It will jump to the position currently occupied by its partner tile's previous spot.
                layer.background.position += new Vector3(layer.tileWidth, 0f, 0f);
            }
            // Check if the tile has moved fully out of view to the right (player moving left).
            else if (distanceToCameraX < -layer.tileWidth)
            {
                // Snap the tile backward by exactly one tile width.
                layer.background.position -= new Vector3(layer.tileWidth, 0f, 0f);
            }
        }

        // 3. Update the camera's position for the next frame's calculation.
        lastCameraPosition = mainCamera.position;
    }
}
