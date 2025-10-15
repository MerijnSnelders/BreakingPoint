using UnityEngine;

public class SpikesFollowingPlayer : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Collider2D spikeTopCollider;

    [Header("Chase Settings")]
    public float speed = 4f;
    public float activateRangeX = 10f;
    public float deactivateRangeX = 12f;

    [Header("Rise Settings")]
    public float riseDistance = 1.5f;
    public float riseSpeed = 3f;
    private bool isRising = false;
    private bool hasRisen = false;
    private Vector2 riseTargetPos;

    [Header("Chase Limits (Markers)")]
    public Transform leftMarker;
    public Transform rightMarker;

    private float leftLimitX;
    private float rightLimitX;

    private Rigidbody2D rb;
    private bool isChasing;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Initialize Rigidbody settings for a stationary, non-gravity object
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.bodyType = RigidbodyType2D.Dynamic; // Start as Dynamic (stationary)

        SetLimitsFromMarkers();
    }

    void FixedUpdate()
    {
        // 1. Check for player reference. Do NOT return if hasRisen is true, as this is when the spike should chase.
        if (player == null) return;

        if (isRising)
        {
            // Move up smoothly (transform-based)
            transform.position = Vector2.MoveTowards(transform.position, riseTargetPos, riseSpeed * Time.fixedDeltaTime);

            if (Mathf.Abs(transform.position.y - riseTargetPos.y) < 0.01f)
            {
                // Rise completed
                isRising = false;
                hasRisen = true;

                // FIX 2: Switch the Rigidbody back to Dynamic so we can move it using velocity (rb.linearVelocity)
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero;
            }
            return; // Exit FixedUpdate while rising, don't execute chase logic yet.
        }

        // 2. Only run the chase logic if the spike has finished rising.
        if (!hasRisen) return;

        // --- CHASE LOGIC ---
        float dx = player.position.x - rb.position.x;
        float absDx = Mathf.Abs(dx);
        bool inZone = player.position.x >= leftLimitX && player.position.x <= rightLimitX;

        // Activation/Deactivation logic based on ranges
        if (!isChasing)
        {
            if (inZone && absDx <= activateRangeX)
                isChasing = true;
        }
        else
        {
            if (!inZone || absDx > deactivateRangeX)
            {
                isChasing = false;
                rb.linearVelocity = Vector2.zero;
            }
        }

        // --- MOVE HORIZONTALLY ---
        if (isChasing)
        {
            float dirX = Mathf.Sign(dx);

            // Check if moving would exceed the set boundaries (bounds check)
            if ((dirX < 0 && rb.position.x <= leftLimitX) ||
                (dirX > 0 && rb.position.x >= rightLimitX))
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                return; // Stop movement at the boundary
            }

            // Apply chasing velocity
            rb.linearVelocity = new Vector2(dirX * speed, rb.linearVelocity.y);
        }
        else
        {
            // If not chasing, stop horizontal movement
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    public void TriggerRise()
    {
        if (hasRisen || isRising) return;

        isRising = true;
        riseTargetPos = new Vector2(transform.position.x, transform.position.y + riseDistance);
        // Set body type to Kinematic to allow controlled movement via transform.position
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void SetLimitsFromMarkers()
    {
        if (leftMarker != null && rightMarker != null)
        {
            // Set limits, ensuring left is always smaller than right
            leftLimitX = Mathf.Min(leftMarker.position.x, rightMarker.position.x);
            rightLimitX = Mathf.Max(leftMarker.position.x, rightMarker.position.x);
        }
    }
}
