using UnityEngine;

public class SimpleSpikeJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float jumpCooldown = 0.25f;
    private float lastJumpTime;

    private Rigidbody2D rb;
    private Transform player;
    private Rigidbody2D playerRb;
    private Collider2D col;
    private Collider2D playerCol;

    private bool playerWasGrounded = true;
    private bool playerJustJumped = false;
    private bool playerWasInRangeBeforeJump = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerRb = playerObj.GetComponent<Rigidbody2D>();
            playerCol = playerObj.GetComponent<Collider2D>();
        }
    }

    private void Update()
    {
        if (player == null || playerRb == null) return;

        float horizontalDistance = Mathf.Abs(transform.position.x - player.position.x);
        bool playerInRange = horizontalDistance <= detectionRange;

        // --- detect player jump start ---
        bool playerGroundedNow = IsPlayerGrounded();
        playerJustJumped = !playerGroundedNow && playerWasGrounded;

        // store range state before jump
        if (playerGroundedNow)
            playerWasInRangeBeforeJump = playerInRange;

        playerWasGrounded = playerGroundedNow;

        bool canJumpNow = Time.time - lastJumpTime > jumpCooldown;

        // ✅ only jump if player started jump while already in range
        if (playerJustJumped && playerWasInRangeBeforeJump && IsGrounded() && canJumpNow)
        {
            Jump();
            lastJumpTime = Time.time;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private bool IsGrounded()
    {
        Bounds b = col.bounds;
        Vector2 boxCenter = new Vector2(b.center.x, b.min.y - 0.05f);
        Vector2 boxSize = new Vector2(b.size.x * 0.9f, 0.05f);
        RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, 0f, groundLayer);

        // debug line
        Color c = hit.collider ? Color.green : Color.red;
        Debug.DrawLine(boxCenter + Vector2.left * (boxSize.x * 0.5f),
                       boxCenter + Vector2.right * (boxSize.x * 0.5f), c);

        return hit.collider != null;
    }

    private bool IsPlayerGrounded()
    {
        if (playerCol != null)
        {
            Bounds pb = playerCol.bounds;
            Vector2 feet = new Vector2(pb.center.x, pb.min.y + 0.01f);
            RaycastHit2D hit = Physics2D.Raycast(feet, Vector2.down, 0.12f, groundLayer);
            return hit.collider != null;
        }
        return Mathf.Abs(playerRb.linearVelocity.y) < 0.01f;
    }
}