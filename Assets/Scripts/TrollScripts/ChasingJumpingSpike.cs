using System.Collections;
using UnityEngine;

public class ChasingJumpingSpike : MonoBehaviour
{
    [Header("Charge Settings")]
    [SerializeField] private float chargeSpeed = 8f;
    [SerializeField] private float chargeDelay = 0.5f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private string playerTag = "Player";

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private float jumpDetectionRange = 8f; // range within which spike reacts to jump

    private Rigidbody2D rb;
    private Collider2D col;
    private Transform player;
    private Rigidbody2D playerRb;
    private Collider2D playerCol;

    private bool isCharging = false;
    private bool playerWasGrounded = true;
    private bool playerJustJumped = false;
    private float chargeDirection = 0f; // -1 = left, +1 = right

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.gravityScale = 2f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            StartCoroutine(ChargeAfterDelay(collision.transform));
        }
    }

    private IEnumerator ChargeAfterDelay(Transform playerTr)
    {
        yield return new WaitForSeconds(chargeDelay);

        // Determine horizontal direction toward player
        chargeDirection = (playerTr.position.x > transform.position.x) ? 1f : -1f;
        isCharging = true;
        rb.linearVelocity = new Vector2(chargeSpeed * chargeDirection, rb.linearVelocity.y);
    }

    private void Update()
    {
        if (player == null || playerRb == null) return;

        // --- Stop charging when hitting a wall ---
        if (isCharging)
        {
            Vector2 dir = new Vector2(chargeDirection, 0f);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.5f, wallLayer);
            if (hit.collider != null)
            {
                StopCharge();
            }
        }

        // --- Player jump detection ---
        bool playerGroundedNow = IsPlayerGrounded();
        playerJustJumped = !playerGroundedNow && playerWasGrounded;
        playerWasGrounded = playerGroundedNow;

        // --- Jump only if player is near ---
        float horizontalDistance = Mathf.Abs(transform.position.x - player.position.x);
        bool playerInRange = horizontalDistance <= jumpDetectionRange;

        // Spike jumps + keeps charging
        if (playerJustJumped && playerInRange && IsGrounded())
        {
            Jump();
        }

        // Keep forward momentum while charging
        if (isCharging && IsGrounded())
        {
            rb.linearVelocity = new Vector2(chargeSpeed * chargeDirection, rb.linearVelocity.y);
        }
    }

    private void Jump()
    {
        // Keep moving horizontally during jump
        rb.linearVelocity = new Vector2(chargeSpeed * chargeDirection, 0f);
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    private bool IsGrounded()
    {
        Bounds b = col.bounds;
        Vector2 origin = new Vector2(b.center.x, b.min.y + 0.05f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawLine(origin, origin + Vector2.down * groundCheckDistance, hit.collider ? Color.green : Color.red);
        return hit.collider != null;
    }

    private bool IsPlayerGrounded()
    {
        if (playerCol != null)
        {
            Bounds b = playerCol.bounds;
            Vector2 feet = new Vector2(b.center.x, b.min.y + 0.01f);
            RaycastHit2D hit = Physics2D.Raycast(feet, Vector2.down, 0.12f, groundLayer);
            return hit.collider != null;
        }
        return Mathf.Abs(playerRb.linearVelocity.y) < 0.01f;
    }

    private void StopCharge()
    {
        rb.linearVelocity = Vector2.zero;
        isCharging = false;
        chargeDirection = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, jumpDetectionRange);
    }
}
