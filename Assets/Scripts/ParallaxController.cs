using UnityEngine;

/// <summary>
/// Controls the movement and infinite looping of a single background tile.
/// NOTE: This script is applied to ONE tile per layer and automatically handles the looping.
/// </summary>
public class ParallaxController : MonoBehaviour
{
    // The initial X position of the tile in the scene (saved in Start).
    private float startPos;

    // The calculated width of the sprite in world units.
    private float length;

    [Header("Setup")]
    [Tooltip("The camera object that will drive the parallax effect.")]
    public GameObject cam;

    [Tooltip("The factor by which this layer moves relative to the camera (0.0=distant, 1.0=foreground).")]
    [Range(0f, 1f)]
    public float parallaxEffect = 0.5f;

    private void Start()
    {
        // Get the initial X position of this tile.
        startPos = transform.position.x;

        // Get the exact width of the SpriteRenderer's bounding box in world units.
        // This is crucial for seamless looping.
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            length = sr.bounds.size.x;
        }
        else
        {
            Debug.LogError("ParallaxController needs a SpriteRenderer on the same GameObject to calculate its length for looping.");
            enabled = false;
        }
    }

    private void LateUpdate() // Changed to LateUpdate for smoother camera-following
    {
        if (cam == null) return;

        // Calculate the distance the camera has moved since the start of the scene, 
        // multiplied by the parallax factor.
        float distanceMoved = cam.transform.position.x * parallaxEffect;

        // Calculate the position where the tile should be.
        // This is the starting position + the parallax movement.
        transform.position = new Vector3(startPos + distanceMoved, transform.position.y, transform.position.z);

        // --- LOOPING LOGIC ---

        // Calculate the actual distance the camera has moved at a 1:1 ratio.
        // This is used to determine when the tile needs to loop.
        float currentCameraMovement = cam.transform.position.x * (1 - parallaxEffect);

        // Check if the camera has moved past the right edge of the current tile.
        if (currentCameraMovement > startPos + length)
        {
            // If the camera moves far enough right, snap the start position forward by one tile length.
            startPos += length;
        }
        // Check if the camera has moved past the left edge of the current tile.
        else if (currentCameraMovement < startPos - length)
        {
            // If the camera moves far enough left, snap the start position backward by one tile length.
            startPos -= length;
        }
    }
}
