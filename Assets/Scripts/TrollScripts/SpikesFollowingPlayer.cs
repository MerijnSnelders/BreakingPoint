using UnityEngine;

public class SpikesFollowingPlayer : MonoBehaviour
{
    [Header("References")]
    public Transform player;                // Assign your player Transform
    public Collider2D spikeTopCollider;     // Optional – used for "player above" check

    [Header("Chase Settings")]
    public float speed = 4f;                // Horizontal chase speed
    public float activateRangeX = 10f;      // How close player must be to start chase
    public float deactivateRangeX = 12f;    // How far player must be to stop chase
    public float minPlayerAboveY = 0f;      // Optional: player must be slightly above spike (set to 0 for ground traps)

    [Header("Chase Limits (Markers)")]
    public Transform leftMarker;            // Empty GameObject marking the left boundary
    public Transform rightMarker;           // Empty GameObject marking the right boundary

    private float leftLimitX;
    private float rightLimitX;

    private Rigidbody2D rb;
    private bool isChasing;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError($"{name}: Missing Rigidbody2D!");

        // Rigidbody2D setup
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        SetLimitsFromMarkers();
    }

    void Start()
    {
        // Ensure limits are updated in case markers moved after Awake
        SetLimitsFromMarkers();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // If markers are moved in editor during play, update limits dynamically
        SetLimitsFromMarkers();

        // --- Distance checks ---
        float dx = player.position.x - rb.position.x;
        float absDx = Mathf.Abs(dx);

        // Optional "player above" logic (usually ignored for ground spikes)
        float spikeTopY = spikeTopCollider != null ? spikeTopCollider.bounds.max.y : transform.position.y;
        float playerFeetY = GetPlayerFeetY();
        bool playerIsAbove = playerFeetY >= spikeTopY + minPlayerAboveY;

        // --- Zone check ---
        bool inZone = player.position.x >= leftLimitX && player.position.x <= rightLimitX;

        // --- CHASE LOGIC ---
        if (!isChasing)
        {
            if (inZone && absDx <= activateRangeX)
            {
                isChasing = true;
                Debug.Log($"{name}: 🟢 CHASE START (player in zone)");
            }
        }
        else
        {
            if (!inZone || absDx > deactivateRangeX)
            {
                isChasing = false;
                rb.linearVelocity = Vector2.zero;
                Debug.Log($"{name}: 🔴 CHASE STOP (out of zone or too far)");
            }
        }

        // --- MOVEMENT ---
        if (isChasing)
        {
            float dirX = Mathf.Sign(dx);

            // Prevent spikes from leaving bounds
            if ((dirX < 0 && rb.position.x <= leftLimitX) ||
                (dirX > 0 && rb.position.x >= rightLimitX))
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            rb.linearVelocity = new Vector2(dirX * speed, 0f);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void SetLimitsFromMarkers()
    {
        if (leftMarker != null && rightMarker != null)
        {
            leftLimitX = Mathf.Min(leftMarker.position.x, rightMarker.position.x);
            rightLimitX = Mathf.Max(leftMarker.position.x, rightMarker.position.x);
        }
    }

    float GetPlayerFeetY()
    {
        if (player != null)
        {
            var col = player.GetComponent<Collider2D>();
            if (col != null)
                return col.bounds.min.y;
        }
        return player != null ? player.position.y - 0.5f : float.NegativeInfinity;
    }

    void OnDrawGizmosSelected()
    {
        SetLimitsFromMarkers();
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftLimitX, transform.position.y + 0.5f, 0f),
                        new Vector3(rightLimitX, transform.position.y + 0.5f, 0f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(leftLimitX, transform.position.y - 0.5f, 0f),
                        new Vector3(rightLimitX, transform.position.y - 0.5f, 0f));
    }

    void OnValidate()
    {
        if (deactivateRangeX < activateRangeX)
            deactivateRangeX = activateRangeX + 0.1f;

        // Update limits when editing in inspector
        SetLimitsFromMarkers();
    }
}
