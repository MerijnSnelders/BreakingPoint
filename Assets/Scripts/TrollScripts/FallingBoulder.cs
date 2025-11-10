using UnityEngine;
using System.Collections;

public class FallingBoulder : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask extraLayers;
    [SerializeField] private float fallSpeed = 8f;
    [SerializeField] private float smashWidth = 1.5f;
    [SerializeField] private float smashDepth = 1.0f;
    [SerializeField] private GameObject destroyEffectPrefab;

    [Header("Optional - Objects that should fall when ground breaks")]
    [SerializeField] private Rigidbody2D[] objectsToFall;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip smashSound;
    [SerializeField, Tooltip("Playback pitch multiplier (1 = normal, <1 = deeper)")]
    private float smashSoundPitch = 0.9f;

    [SerializeField, Tooltip("Start time (in seconds) for playback within the clip")]
    private float playStartTime = 1f;

    [SerializeField, Tooltip("End time (in seconds) for playback within the clip")]
    private float playEndTime = 4f;

    private Rigidbody2D rb;
    private LayerMask combinedLayerMask;
    private bool hasPlayedSound = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;

        int groundLayer = LayerMask.NameToLayer("Ground");
        combinedLayerMask = extraLayers | (1 << groundLayer);
    }

    public void Drop() => rb.gravityScale = fallSpeed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 🎯 Trigger sound immediately when collider touches something valid
        if (!hasPlayedSound && IsInCombinedLayer(collision.gameObject.layer))
        {
            hasPlayedSound = true;

            // 🎵 Play only between 1s and 4s of the sound
            if (audioSource != null && smashSound != null)
                StartCoroutine(PlaySoundSegment(audioSource, smashSound, smashSoundPitch, playStartTime, playEndTime));

            MakeAssignedObjectsFall();
            SmashGroundInstantly();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsInCombinedLayer(collision.gameObject.layer))
            SmashGroundInstantly();
    }

    private bool IsInCombinedLayer(int layer)
    {
        return (combinedLayerMask & (1 << layer)) != 0;
    }

    private void SmashGroundInstantly()
    {
        float boulderHeight = 1f;
        Collider2D boulderCollider = GetComponent<Collider2D>();
        if (boulderCollider != null)
            boulderHeight = boulderCollider.bounds.size.y;

        float offset = boulderHeight / 2f + smashDepth / 2f;
        Vector2 boxCenter = new Vector2(transform.position.x, transform.position.y - offset);
        Vector2 boxSize = new Vector2(smashWidth, smashDepth);

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f, combinedLayerMask);
        bool playerNearby = false;

        foreach (var hit in hits)
        {
            if (hit == null) continue;
            if (hit.gameObject == gameObject) continue;

            if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                playerNearby = true;
                continue;
            }

            if (destroyEffectPrefab != null)
            {
                GameObject fx = Instantiate(destroyEffectPrefab, hit.transform.position, Quaternion.identity);
                Destroy(fx, 1f);
            }

            Destroy(hit.gameObject);
        }

        if (playerNearby)
        {
            PlayerDeath player = FindObjectOfType<PlayerDeath>();
            if (player != null)
                player.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
        }

        rb.gravityScale = fallSpeed;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.freezeRotation = true;
    }

    private void MakeAssignedObjectsFall()
    {
        foreach (Rigidbody2D obj in objectsToFall)
            if (obj != null)
                obj.bodyType = RigidbodyType2D.Dynamic;
    }

    // 🎧 Helper coroutine — plays only from `startTime` to `endTime` in clip
    private IEnumerator PlaySoundSegment(AudioSource source, AudioClip clip, float pitch, float startTime, float endTime)
    {
        source.pitch = pitch;
        source.clip = clip;

        source.time = Mathf.Clamp(startTime, 0f, clip.length - 0.1f); // skip to the desired second
        source.Play();

        float duration = Mathf.Clamp(endTime - startTime, 0.1f, clip.length - startTime);
        yield return new WaitForSeconds(duration);

        source.Stop();
        source.pitch = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float boulderHeight = 1f;
        Collider2D boulderCollider = GetComponent<Collider2D>();
        if (boulderCollider != null)
            boulderHeight = boulderCollider.bounds.size.y;

        float offset = boulderHeight / 2f + smashDepth / 2f;
        Vector3 boxCenter = new Vector3(transform.position.x, transform.position.y - offset, 0f);
        Vector3 boxSize = new Vector3(smashWidth, smashDepth, 0.1f);

        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
