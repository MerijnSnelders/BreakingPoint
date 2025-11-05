using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [Header("Death Settings")]
    [SerializeField] private float fallThreshold = -10f;
    [SerializeField] private LayerMask deadlyLayer;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;

    private Animator animator;
    private bool isDying = false;
    private Rigidbody2D rb;
    private PlayerMovementScript movementScript;

    // ✅ Checkpoint system
    private static Vector2 respawnPoint;
    private static bool hasCheckpoint = false;
    private static bool diedOnce = false;

    // ✅ Checkpoint sound persistence
    private static bool checkpointSoundPlayed = false;
    private static string lastSceneName = null;

    // falling back to other level
    public bool ResetLevelFallTroll = false;
    public bool isLevel3 = false;
    private float newFallTreshhold = -99f;



    void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // ✅ Only reset checkpoint sound if a *different* level is loaded
        if (lastSceneName != currentScene)
        {
            checkpointSoundPlayed = false;
            lastSceneName = currentScene;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<PlayerMovementScript>();

        // ✅ Move to checkpoint if we’ve died once and have one saved
        if (diedOnce && hasCheckpoint)
        {
            transform.position = respawnPoint;
        }
        else
        {
            respawnPoint = transform.position;
        }
    }

    private void Update()
    {
        if (isDying) return;

        if(isLevel3)
        {
            if (transform.position.y < newFallTreshhold)
                Die();
        }
        else
        {
            if (transform.position.y < fallThreshold)
                Die();
        }

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
        DeathCounter.Instance?.AddDeath();
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
        diedOnce = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ✅ Checkpoint setter
    public void SetCheckpoint(Vector2 newCheckpoint)
    {
        respawnPoint = newCheckpoint;
        hasCheckpoint = true;
        Debug.Log("Checkpoint set to: " + newCheckpoint);
    }

    // ✅ Checkpoint sound management
    public static bool HasCheckpointSoundPlayed() => checkpointSoundPlayed;
    public static void MarkCheckpointSoundPlayed() => checkpointSoundPlayed = true;
}
