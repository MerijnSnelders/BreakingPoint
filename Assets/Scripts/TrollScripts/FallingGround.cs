using UnityEngine;

public class FallingGround : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool hasFallen = false;

    [Header("Settings")]
    public float fallGravity = 5f;        // how fast it falls
    public float requiredCenterMargin = 0.1f; // how centered the player must be

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            Debug.LogError($"{name}: Missing Rigidbody2D!");

        rb.bodyType = RigidbodyType2D.Kinematic; // starts frozen
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log($"{name}: Colliding with {collision.collider.name} (Tag: {collision.collider.tag})");


        // Don’t trigger again
        if (hasFallen) return;

        // Only react to player
        if (!collision.collider.CompareTag("Player")) return;

        // Get collider bounds
        Bounds playerBounds = collision.collider.bounds;
        Bounds platformBounds = GetComponent<Collider2D>().bounds;

        // ✅ Check if the player's whole width is within the platform
        bool fullyOnPlatform =
            playerBounds.min.x > platformBounds.min.x + requiredCenterMargin &&
            playerBounds.max.x < platformBounds.max.x - requiredCenterMargin &&
            playerBounds.min.y >= platformBounds.max.y - 0.05f; // small tolerance

        if (fullyOnPlatform)
        {
            FallNow();
        }
    }

    private void FallNow()
    {
        hasFallen = true;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        // Optional: disable collision with ground layer so it doesn't get stuck
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer >= 0)
            Physics2D.IgnoreLayerCollision(gameObject.layer, groundLayer, true);

        Debug.Log($"{name}: Player fully on top — falling now!");
    }
}
