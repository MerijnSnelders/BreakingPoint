using UnityEngine;

public class FallingGround : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool hasFallen = false;

    [Header("Settings")]
    public float fallGravity = 5f;
    [Range(0f, 1f)]
    public float requiredCoverage = 1f;
    public float requiredCenterMargin = 0.1f; // tolerance margin

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fallSound;
    [SerializeField, Tooltip("Playback pitch multiplier (1 = normal, >1 = faster)")]
    private float fallSoundPitch = 1.1f;
    [SerializeField, Tooltip("Stop sound after this many seconds (0 = play full clip)")]
    private float fallSoundDuration = 0.8f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError($"{name}: Missing Rigidbody2D!");

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (hasFallen) return;
        if (!collision.collider.CompareTag("Player")) return;

        Bounds player = collision.collider.bounds;
        Bounds platform = GetComponent<Collider2D>().bounds;

        float overlapMin = Mathf.Max(player.min.x, platform.min.x);
        float overlapMax = Mathf.Min(player.max.x, platform.max.x);
        float overlapWidth = Mathf.Max(0, overlapMax - overlapMin);

        float coverage = overlapWidth / player.size.x;
        bool enoughCoverage = coverage >= requiredCoverage;
        bool onTop = player.min.y >= platform.max.y - 0.05f;

        if (enoughCoverage && onTop)
            FallNow();
    }

    private void FallNow()
    {
        hasFallen = true;

        // 🎵 Play fall sound (shortened & slightly faster)
        if (audioSource != null && fallSound != null)
            StartCoroutine(PlayShortenedSound(fallSound, fallSoundPitch, fallSoundDuration));

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer >= 0)
            Physics2D.IgnoreLayerCollision(gameObject.layer, groundLayer, true);

        Debug.Log($"{name}: Player coverage met — falling now!");
    }

    // 🎧 Helper coroutine for faster & shortened playback
    private System.Collections.IEnumerator PlayShortenedSound(AudioClip clip, float pitch, float duration)
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
