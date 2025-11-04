using UnityEngine;

public class SpikesFollowingPlayer : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Rise Settings")]
    public float riseDistance = 1.5f;
    public float riseSpeed = 3f;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    public Transform leftBorder;
    public Transform rightBorder;

    private Rigidbody2D rb;

    // State
    private bool isRising = false;
    private bool hasRisen = false;
    private bool isChasing = false;
    private bool playerInChaseZone = false;

    private Vector2 riseTargetPos;
    private float riseFinishTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;

        // auto-find player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        // --- Rising phase ---
        if (isRising)
        {
            transform.position = Vector2.MoveTowards(transform.position, riseTargetPos, riseSpeed * Time.fixedDeltaTime);
            if (Mathf.Abs(transform.position.y - riseTargetPos.y) < 0.01f)
            {
                isRising = false;
                hasRisen = true;
                riseFinishTime = Time.time;
                Debug.Log($"[{name}] Finished rising!");

                // ✅ If player already in chase zone, start chasing immediately
                if (playerInChaseZone)
                {
                    isChasing = true;
                    Debug.Log($"[{name}] Player already above — start chasing immediately!");
                }
            }
            return;
        }

        // --- Chasing movement ---
        if (isChasing)
        {
            float dx = player.position.x - rb.position.x;
            float dirX = Mathf.Sign(dx);

            // Stop if reaching borders
            if ((dirX < 0 && rb.position.x <= leftBorder.position.x) ||
                (dirX > 0 && rb.position.x >= rightBorder.position.x))
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            rb.linearVelocity = new Vector2(dirX * chaseSpeed, 0f);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Called when the player enters proximity trigger
    public void TriggerRise()
    {
        if (isRising || hasRisen) return;

        isRising = true;
        riseTargetPos = new Vector2(transform.position.x, transform.position.y + riseDistance);
        Debug.Log($"[{name}] Rising!");
    }

    // Called by chase trigger when player jumps above
    public void StartChasing()
    {
        playerInChaseZone = true; // remember that the player is above

        if (hasRisen)
        {
            isChasing = true;
            Debug.Log($"[{name}] START chasing!");
        }
        else
        {
            Debug.Log($"[{name}] Player above, will start chasing after rising.");
        }
    }

    // Called when player leaves chase zone
    public void StopChasing()
    {
        playerInChaseZone = false;
        isChasing = false;
        Debug.Log($"[{name}] STOP chasing!");
    }
}
