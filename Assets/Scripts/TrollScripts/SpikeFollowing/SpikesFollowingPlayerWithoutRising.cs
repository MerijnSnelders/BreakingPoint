using UnityEngine;

public class SpikesFollowingPlayerWithoutRising : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    public Transform leftBorder;
    public Transform rightBorder;

    private Rigidbody2D rb;
    private bool isChasing = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;

        // Auto-find player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        if (isChasing && player != null)
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

    public void StartChasing()
    {
        isChasing = true;
        Debug.Log($"[{name}] START chasing!");
    }

    public void StopChasing()
    {
        isChasing = false;
        Debug.Log($"[{name}] STOP chasing!");
    }
}
