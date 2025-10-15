using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FleeingPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;       // assign Player
    [SerializeField] private Transform rightLimit;   // assign invisible marker

    [Header("Movement Settings")]
    [SerializeField] private float detectionRange = 5f; // how close player must get
    [SerializeField] private float moveSpeed = 10f;     // speed to the right
    [SerializeField] private bool returnToStart = false;
    [SerializeField] private float returnSpeed = 3f;

    private Rigidbody2D rb;
    private Vector3 startPos;
    private bool isMovingRight;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        startPos = transform.position;
    }

    void Update()
    {
        if (player == null || rightLimit == null) return;

        float distance = Vector2.Distance(player.position, transform.position);

        if (distance < detectionRange)
            isMovingRight = true;
        else if (returnToStart)
            isMovingRight = false;
    }

    void FixedUpdate()
    {
        if (rb == null || rightLimit == null) return;

        float limitX = rightLimit.position.x;

        if (isMovingRight)
        {
            // Move right until reaching the limit
            if (transform.position.x < limitX)
            {
                Vector2 move = Vector2.right * moveSpeed * Time.fixedDeltaTime;
                rb.MovePosition((Vector2)transform.position + move);
            }
            else
            {
                // Stop exactly at the limit
                Vector3 clamped = transform.position;
                clamped.x = limitX;
                rb.MovePosition(clamped);
            }
        }
        else if (returnToStart)
        {
            // Smooth return
            Vector2 dir = ((Vector2)startPos - (Vector2)transform.position);
            if (dir.magnitude > 0.05f)
            {
                Vector2 move = dir.normalized * returnSpeed * Time.fixedDeltaTime;
                rb.MovePosition((Vector2)transform.position + move);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw line to right limit if assigned
        if (rightLimit != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, rightLimit.position);
            Gizmos.DrawSphere(rightLimit.position, 0.1f);
        }
    }
}
