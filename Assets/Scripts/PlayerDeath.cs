using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float fallThreshold = -10f; // Y position below which player dies
    [SerializeField] private LayerMask deadlyLayer; // assign DeadlyObstacle layer in inspector

    private Animator animator;
    private bool isDying = false;
    private Rigidbody2D rb;
    private PlayerMovementScript movementScript; // reference to your movement controller

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<PlayerMovementScript>();
    }

    private void Update()
    {
        if (isDying) return; // prevent dying twice

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

        // 🔒 Lock player movement
        if (movementScript != null)
            movementScript.CanMove = false;

        // 🧊 Stop physics movement
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // 🎲 Pick random death animation
        int randomDeath = Random.Range(1, 3); // chooses 1 or 2
        animator.SetTrigger("Die" + randomDeath);
        Debug.Log("Die" + randomDeath);

        // ⏳ Wait until animation is done, then reload
        float animLength = GetAnimationClipLength("Death" + randomDeath) + 0.75f;
        Invoke(nameof(ReloadScene), animLength);
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

    private void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
