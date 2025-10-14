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

        // --- Get positions for checks ---
        float playerY = other.bounds.center.y;
        float blockY = blockRenderer.bounds.center.y;
        float playerX = other.bounds.center.x;
        float blockX = blockRenderer.bounds.center.x;

        // ✅ NEW: only trigger if player is roughly below the block horizontally
        float horizontalTolerance = blockRenderer.bounds.extents.x * 1f;
        bool isUnderHorizontally = Mathf.Abs(playerX - blockX) < horizontalTolerance;

        // ✅ Only trigger if player is below, under horizontally, and moving upward
        if (playerRb.linearVelocity.y > 0f && playerY < blockY && isUnderHorizontally)
        {
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
}
