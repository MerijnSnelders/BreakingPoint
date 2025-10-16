using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float fallThreshold = -10f;
    [SerializeField] private LayerMask deadlyLayer;

    private Animator animator;
    private bool isDying = false;
    private Rigidbody2D rb;
    private PlayerMovementScript movementScript;

    // ✅ Add this:
    private Vector2 respawnPoint;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<PlayerMovementScript>();

        // Set default respawn point at the player’s start position
        respawnPoint = transform.position;
    }

    private void Update()
    {
        if (isDying) return;

        if (transform.position.y < fallThreshold)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDying) return;

        if (((1 << collision.gameObject.layer) & deadlyLayer) != 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDying) return;

        if (((1 << collision.gameObject.layer) & deadlyLayer) != 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDying) return;
        isDying = true;

        Debug.Log("Player Died!");

        if (movementScript != null)
            movementScript.CanMove = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        int randomDeath = Random.Range(1, 3);
        animator.SetTrigger("Die" + randomDeath);

        float animLength = GetAnimationClipLength("Death" + randomDeath) + 0.75f;
        Invoke(nameof(RespawnAtCheckpoint), animLength);
    }

    private float GetAnimationClipLength(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 1f;
    }

    // ✅ New respawn logic
    private void RespawnAtCheckpoint()
    {
        Debug.Log("Respawning at checkpoint...");

        // Reset death state
        isDying = false;

        // Re-enable movement
        if (movementScript != null)
            movementScript.CanMove = true;

        // Re-enable physics
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 1f;

        // Move player to checkpoint
        transform.position = respawnPoint;
    }

    // ✅ Public setter for checkpoint
    public void SetCheckpoint(Vector2 newCheckpoint)
    {
        respawnPoint = newCheckpoint;
        Debug.Log("Checkpoint set to: " + newCheckpoint);
    }
}
