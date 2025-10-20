using UnityEngine;

public class FallingGround : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool hasFallen = false;

    [Header("Settings")]
    public float fallGravity = 5f;
    [Range(0f, 1f)]
    public float requiredCoverage = 1f;
    // 1.0 = player must be fully on top
    // 0.5 = half of player width must be on top
    // 0.1 = only small part must be on top

    public float requiredCenterMargin = 0.1f; // tolerance margin

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError($"{name}: Missing Rigidbody2D!");

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (hasFallen) return;
        if (!collision.collider.CompareTag("Player")) return;

        Bounds player = collision.collider.bounds;
        Bounds platform = GetComponent<Collider2D>().bounds;

        // Calculate how much of the player’s width overlaps with the platform
        float overlapMin = Mathf.Max(player.min.x, platform.min.x);
        float overlapMax = Mathf.Min(player.max.x, platform.max.x);
        float overlapWidth = Mathf.Max(0, overlapMax - overlapMin);

        float coverage = overlapWidth / player.size.x; // percentage of player width supported

        bool enoughCoverage = coverage >= requiredCoverage;
        bool onTop = player.min.y >= platform.max.y - 0.05f;

        if (enoughCoverage && onTop)
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

        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer >= 0)
            Physics2D.IgnoreLayerCollision(gameObject.layer, groundLayer, true);

        Debug.Log($"{name}: Player coverage met — falling now!");
    }
}
