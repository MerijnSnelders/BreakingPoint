using System.Collections;
using UnityEngine;

public class ChainFallingPlatforms : MonoBehaviour
{
    [Header("Platforms to Drop (in order)")]
    [SerializeField] private GameObject[] platforms;

    [Header("Timing Settings")]
    [Tooltip("Delay before first platform starts falling")]
    [SerializeField] private float startDelay = 0f;

    [Tooltip("Delay between each platform drop")]
    [SerializeField] private float delayBetweenPlatforms = 1f;

    [Tooltip("How long before a platform disappears after starting to fall")]
    [SerializeField] private float destroyDelay = 3f;

    [Header("Physics Settings")]
    [Tooltip("Gravity scale applied when platform starts falling")]
    [SerializeField] private float gravityScale = 2f;

    [Tooltip("Should the platforms reset after falling?")]
    [SerializeField] private bool resetAfterFall = false;

    [Tooltip("Delay before reset (if enabled)")]
    [SerializeField] private float resetDelay = 5f;

    private Vector3[] initialPositions;
    private Rigidbody2D[] rigidbodies;

    private bool triggered = false;

    private void Start()
    {
        // Store initial positions & rigidbodies
        initialPositions = new Vector3[platforms.Length];
        rigidbodies = new Rigidbody2D[platforms.Length];

        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i] != null)
            {
                initialPositions[i] = platforms[i].transform.position;
                rigidbodies[i] = platforms[i].GetComponent<Rigidbody2D>();
                if (rigidbodies[i] == null)
                {
                    rigidbodies[i] = platforms[i].AddComponent<Rigidbody2D>();
                }
                rigidbodies[i].bodyType = RigidbodyType2D.Kinematic;
                rigidbodies[i].freezeRotation = true;
            }
        }
    }

    // Trigger the chain drop (can be called from a trigger zone or manually)
    public void ActivateFallSequence()
    {
        if (!triggered)
            StartCoroutine(FallSequence());
    }

    private IEnumerator FallSequence()
    {
        triggered = true;

        yield return new WaitForSeconds(startDelay);

        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i] != null)
            {
                StartCoroutine(FallPlatform(rigidbodies[i], platforms[i]));
                yield return new WaitForSeconds(delayBetweenPlatforms);
            }
        }

        // Optional: Reset platforms after some time
        if (resetAfterFall)
        {
            yield return new WaitForSeconds(resetDelay);
            ResetPlatforms();
            triggered = false;
        }
    }

    private IEnumerator FallPlatform(Rigidbody2D rb, GameObject platform)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityScale;

        yield return new WaitForSeconds(destroyDelay);

        if (!resetAfterFall)
            Destroy(platform);
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void ResetPlatforms()
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i] != null)
            {
                platforms[i].transform.position = initialPositions[i];
                rigidbodies[i].bodyType = RigidbodyType2D.Kinematic;
                rigidbodies[i].linearVelocity = Vector2.zero;
            }
        }
    }

    // Optional: trigger from a collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            ActivateFallSequence();
    }
}

