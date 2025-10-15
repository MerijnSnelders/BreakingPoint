using UnityEngine;

/// <summary>
/// Manages the falling mechanics and self-destruction of a trap spike.
/// This object should be placed on the "DeadlyObstacle" layer to kill the player.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class FallingSpike : MonoBehaviour
{
    [Header("Fall Settings")]
    [Tooltip("The gravity multiplier applied when the spike starts falling.")]
    [SerializeField] private float fallGravityScale = 5f;
    [Tooltip("Time before the spike is destroyed after its first contact.")]
    [SerializeField] private float destroyDelay = 0.1f;

    private Rigidbody2D rb;
    private bool isFalling = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure the spike is stationary and not affected by physics until triggered
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    /// <summary>
    /// Called by the trigger script to initiate the fall.
    /// </summary>
    public void TriggerFall()
    {
        if (isFalling) return;

        isFalling = true;
        // Switch to Dynamic body type and apply gravity to start the fall instantly
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravityScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Spikes should only react to collisions if they are actively falling
        if (!isFalling) return;

        // Once the falling spike hits anything (ground, wall, player, etc.), it self-destructs.
        // We use a small delay (destroyDelay) to ensure the player's death/damage logic 
        // (which runs on collision) has time to register before the spike is removed.
        Destroy(gameObject, destroyDelay);
    }
}
