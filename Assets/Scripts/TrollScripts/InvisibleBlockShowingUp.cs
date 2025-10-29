using System.Collections;
using UnityEngine;

public class InvisibleBlockShowingUp : MonoBehaviour
{
    [Header("References")]
    public Collider2D blockCollider;      // assign the block’s collider
    public SpriteRenderer blockRenderer;  // assign the block’s sprite

    private bool isActive = false;

    [Header("Pop-Up Settings")]
    public float popHeight = 0f;
    public float popSpeed = 5f;

    [Header("Trap Settings")]
    [Tooltip("If checked, the block breaks/disappears when stepped on by the player.")]
    public bool breakWhenSteppedOn = false;
    [Tooltip("How long before the block disappears after the player steps on it.")]
    public float breakDelay = 0f;

    private void Start()
    {
        // Hide and disable at start
        if (blockCollider != null) blockCollider.enabled = false;
        if (blockRenderer != null) blockRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive) return;
        if (!other.CompareTag("Player")) return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        float playerY = other.bounds.center.y;
        float blockY = blockRenderer.bounds.center.y;
        float playerX = other.bounds.center.x;
        float blockX = blockRenderer.bounds.center.x;

        float horizontalTolerance = blockRenderer.bounds.extents.x * 1.6f;
        bool isUnderHorizontally = Mathf.Abs(playerX - blockX) < horizontalTolerance;
        bool isBelowBlock = playerY < (blockY - 0.1f);
        bool movingUpwardEnough = playerRb.linearVelocity.y > -1f;

        // ✅ Only trigger from below
        if (isBelowBlock && isUnderHorizontally && movingUpwardEnough)
        {
            Vector2 newVelocity = playerRb.linearVelocity;
            newVelocity.y = -4f;
            playerRb.linearVelocity = newVelocity;

            StartCoroutine(ActivateBlock());
        }
    }

    private IEnumerator ActivateBlock()
    {
        isActive = true;

        blockCollider.enabled = true;
        blockRenderer.enabled = true;

        Vector3 startPos = blockRenderer.transform.position;
        Vector3 endPos = startPos + Vector3.up * popHeight;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * popSpeed;
            blockRenderer.transform.position = Vector3.Lerp(
                startPos,
                endPos,
                Mathf.Sin(t * Mathf.PI * 0.5f)
            );
            yield return null;
        }

        blockRenderer.transform.position = endPos;
    }

    // ✅ NEW: detects if player steps *on top* of an active block
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive || !breakWhenSteppedOn) return;
        if (!collision.collider.CompareTag("Player")) return;

        // Check if the player is above the block
        ContactPoint2D contact = collision.contacts[0];
        if (contact.point.y > blockRenderer.bounds.center.y)
        {
            StartCoroutine(BreakBlock());
        }
    }

    private IEnumerator BreakBlock()
    {
        yield return new WaitForSeconds(breakDelay);

        if (blockCollider != null) blockCollider.enabled = false;

        // Optional: small fade-out effect
        if (blockRenderer != null)
        {
            Color c = blockRenderer.color;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 3f;
                c.a = Mathf.Lerp(1f, 0f, t);
                blockRenderer.color = c;
                yield return null;
            }
        }

        gameObject.SetActive(false); // hide completely
    }
}
