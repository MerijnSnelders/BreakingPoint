using System.Collections;
using UnityEngine;

public class FallingGround_Delayed : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isTriggered = false;

    [Header("Settings")]
    public float fallGravity = 5f;       // how fast it falls
    public float delayBeforeFall = 0.5f; // seconds before it drops

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fallSound;
    [SerializeField, Tooltip("Playback pitch multiplier (1 = normal, >1 = faster)")]
    private float fallSoundPitch = 1.1f;
    [SerializeField, Tooltip("Stop sound after this many seconds (0 = play full clip)")]
    private float fallSoundDuration = 0.8f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            Debug.LogError($"{name}: Missing Rigidbody2D!");

        rb.bodyType = RigidbodyType2D.Kinematic; // starts frozen
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isTriggered) return;
        if (!collision.collider.CompareTag("Player")) return;

        isTriggered = true;
        StartCoroutine(FallAfterDelay());
    }

    private IEnumerator FallAfterDelay()
    {
        // 🎵 Play the crumble sound during the delay
        if (audioSource != null && fallSound != null)
            StartCoroutine(PlayShortenedSound(fallSound, fallSoundPitch, fallSoundDuration));

        yield return new WaitForSeconds(delayBeforeFall);

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        // Optional: ignore ground layer collisions
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer >= 0)
            Physics2D.IgnoreLayerCollision(gameObject.layer, groundLayer, true);

        Debug.Log($"{name}: Delay finished — falling now!");
    }

    // 🎧 Helper coroutine for faster & shortened playback
    private IEnumerator PlayShortenedSound(AudioClip clip, float pitch, float duration)
    {
        audioSource.pitch = pitch;
        audioSource.clip = clip;
        audioSource.Play();
        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
            audioSource.Stop();
        }
        audioSource.pitch = 1f;
    }
}
