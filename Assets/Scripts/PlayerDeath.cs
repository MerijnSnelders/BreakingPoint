using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float fallThreshold = -10f;
    [SerializeField] private LayerMask deadlyLayer;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;


    private Animator animator;
    private bool isDying = false;
    private Rigidbody2D rb;
    private PlayerMovementScript movementScript;

    private static Vector2 respawnPoint;
    private static bool hasCheckpoint = false;
    private static bool diedOnce = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<PlayerMovementScript>();

        // ✅ Only move to checkpoint if we already died before
        if (diedOnce && hasCheckpoint)
        {
            transform.position = respawnPoint;
        }
        else
        {
            // Start at natural starting point
            respawnPoint = transform.position;
        }
    }

    private void Update()
    {
        if (isDying) return;

        if (transform.position.y < fallThreshold)
            Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDying) return;

        if (((1 << collision.gameObject.layer) & deadlyLayer) != 0)
            Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDying) return;

        if (((1 << collision.gameObject.layer) & deadlyLayer) != 0)
            Die();
    }

    private void Die()
    {
        if (isDying) return;
        isDying = true;

        if (movementScript != null)
            movementScript.CanMove = false;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        int randomDeath = Random.Range(1, 3);
        animator.SetTrigger("Die" + randomDeath);

        float animLength = GetAnimationClipLength("Death" + randomDeath) + 0.75f;
        Invoke(nameof(ReloadScene), animLength);
    }

    private float GetAnimationClipLength(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 1f;
    }

    private void ReloadScene()
    {
        diedOnce = true; // ✅ mark that we've died once
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ✅ Checkpoint setter
    public void SetCheckpoint(Vector2 newCheckpoint)
    {
        respawnPoint = newCheckpoint;
        hasCheckpoint = true;
        Debug.Log("Checkpoint set to: " + newCheckpoint);
    }
}
