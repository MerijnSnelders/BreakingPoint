using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player;          // Assign your player in the inspector
    public float fixedY = 0f;         // The Y position the camera will stay at
    public float fixedStartX = 0f;    // <--- NEW: The camera's locked START X position
    public float smoothSpeed = 0.125f; // How quickly the camera moves to the target


    // Private variable to track if the camera has started following the player
    private bool isFollowing = false;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned in the Inspector!");
            return;
        }

        // CRITICAL FIX: Set the camera to the fixed starting X position
        // regardless of where the player starts.
        transform.position = new Vector3(fixedStartX, fixedY, transform.position.z);
    }

    void LateUpdate()
    {
        if (player == null) return;

        // --- 1. Dead Zone Check (Lock the Camera) ---
        if (!isFollowing)
        {
            // Calculate the horizontal distance between the player and the camera's fixed start X position
            float distanceToStart = player.position.x - fixedStartX;
            Debug.Log(distanceToStart);

            if (distanceToStart >= 0)
            {
                // Player has moved far enough from the starting point, begin following
                isFollowing = true;
            }
            else
            {
                // Stay locked at the fixed starting position until the threshold is met
                // No movement needed here, as it was set in Start().
                return;
            }
        }

        // --- 2. Follow the player (Once isFollowing is true) ---

        // The target is the player's X, the fixed Y, and the camera's original Z
        Vector3 targetPosition = new Vector3(player.position.x, fixedY, transform.position.z);

        // Smoothly move the camera toward the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        if (transform.position.x <= fixedStartX)
        {
            isFollowing = false;
        }
    }
}