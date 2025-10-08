using System.Collections;
using UnityEngine;

public class DisappearingPlatformScript : MonoBehaviour
{
    [Header("Force Walls")]
    public Collider2D leftWall;
    public Collider2D rightWall;
    public float wallDuration = 1f; // how long walls stay active

    [Header("Trigger")]
    public Collider2D activationTrigger; // assign the trigger collider here

    [Header("Disappear Settings")]
    public float disappearDelay = 0.3f;  // how long before it disappears
    public float reappearDelay = 3f;     // optional, set 0 to never reappear

    private bool hasDisappeared = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();

        // ensure walls are off at start
        if (leftWall != null) leftWall.enabled = false;
        if (rightWall != null) rightWall.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDisappeared) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(DisappearAndActivateWalls());
        }
    }

    private IEnumerator DisappearAndActivateWalls()
    {
        hasDisappeared = true;

        // small delay before disappearing
        yield return new WaitForSeconds(disappearDelay);

        // make it disappear
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (platformCollider != null) platformCollider.enabled = false;

        // activate walls
        if (leftWall != null) leftWall.enabled = true;
        if (rightWall != null) rightWall.enabled = true;

        yield return new WaitForSeconds(wallDuration);

        if (leftWall != null) leftWall.enabled = false;
        if (rightWall != null) rightWall.enabled = false;

        // reappear if enabled
        if (reappearDelay > 0)
        {
            yield return new WaitForSeconds(reappearDelay);
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            if (platformCollider != null) platformCollider.enabled = true;
            hasDisappeared = false;
        }
        else
        {
            // permanently gone
            Destroy(gameObject);
        }
    }
}
