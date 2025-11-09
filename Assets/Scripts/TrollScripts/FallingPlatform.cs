using UnityEngine;

/// <summary>
/// This script goes on the platform prefab.
/// It handles the platform's movement, speeding up, and self-destruction.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FallingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The normal downward speed of the platform.")]
    public float normalSpeed = 1.5f;

    [Tooltip("The faster speed when the player is on it.")]
    public float fastSpeed = 4.0f;

    [Tooltip("The Y position at which this platform will be destroyed.")]
    public float destroyYLevel = -5.0f;

    private Rigidbody2D rb;
    private bool playerIsOn = false;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Set to Kinematic so it doesn't fall with gravity and we can control it via script.
        rb.isKinematic = true;
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        // Move the platform down
        rb.linearVelocity = new Vector2(0, -currentSpeed);

        // Check if the platform has reached the deletion level
        if (transform.position.y <= destroyYLevel)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called by Unity when another collider enters this object's collider.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Check if the object that collided is the "Player"
        // (Make sure your player object has the tag "Player")
        if (other.gameObject.CompareTag("Player"))
        {
            // We can also check if the player landed on *top* of the platform
            bool playerIsOnTop = false;
            foreach (ContactPoint2D contact in other.contacts)
            {
                // contact.normal.y < -0.5f means the collision came from above
                if (contact.normal.y < -0.5f)
                {
                    playerIsOnTop = true;
                    break;
                }
            }

            if (playerIsOnTop)
            {
                Debug.Log("Player is on the platform! Falling faster.");
                playerIsOn = true;
                currentSpeed = fastSpeed;
            }
        }
    }

    /// <summary>
    /// Optional: If you want the platform to slow down if the player jumps off.
    /// If you want it to fall fast forever once touched, just delete this function.
    /// </summary>
}