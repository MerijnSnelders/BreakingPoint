using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeChaseScript : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 10f;        // Horizontal chase speed
    [SerializeField] private float chaseDelay = 0.2f;       // Delay after trigger before chase starts
    [SerializeField] private float wallCheckDistance = 0.5f;// Ray distance to detect wall
    [SerializeField] private LayerMask wallLayer;           // Walls that stop spikes
    [SerializeField] private string playerTag = "Player";

    [Header("Spikes to Control")]
    [Tooltip("Assign spike Rigidbody2D here. If empty and autoFindByTag is true, we'll search by tag.")]
    [SerializeField] private Rigidbody2D[] spikes;

    [Header("Auto-Find (optional)")]
    [SerializeField] private bool autoFindByTag = true;
    [SerializeField] private string spikeTag = "Spike";     // Tag on spike GameObjects
    [SerializeField] private float findRadius = 100f;       // Radius from this trigger to find spikes

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private Transform player;
    private bool chasing = false;
    private float startChaseTime = -1f;
    private readonly List<Rigidbody2D> activeSpikes = new();

    private void Reset()
    {
        // Ensure this collider is a trigger
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void Start()
    {
        // Grab player
        var playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null) player = playerObj.transform;

        // Build spike list
        activeSpikes.Clear();

        if (spikes != null && spikes.Length > 0)
        {
            foreach (var rb in spikes) if (rb != null) activeSpikes.Add(rb);
        }
        else if (autoFindByTag)
        {
            var all = GameObject.FindGameObjectsWithTag(spikeTag);
            foreach (var go in all)
            {
                if (Vector2.Distance(transform.position, go.transform.position) <= findRadius)
                {
                    var rb = go.GetComponent<Rigidbody2D>();
                    if (rb != null) activeSpikes.Add(rb);
                }
            }
            if (debugLogs) Debug.Log($"[MultiSpikeChaseTrigger] Auto-found {activeSpikes.Count} spikes by tag '{spikeTag}'.");
        }

        if (activeSpikes.Count == 0 && debugLogs)
            Debug.LogWarning("[MultiSpikeChaseTrigger] No spikes assigned/found. Assign Rigidbody2D references or enable autoFindByTag.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (player == null) return;
        if (!other.CompareTag(playerTag)) return;

        chasing = true;
        startChaseTime = Time.time;

        if (debugLogs) Debug.Log("[MultiSpikeChaseTrigger] Player entered trigger → starting chase (after delay).");
    }

    private void Update()
    {
        if (!chasing || player == null) return;
        if (Time.time < startChaseTime + chaseDelay) return; // wait out the delay

        // Drive each spike toward the player's current X position
        for (int i = activeSpikes.Count - 1; i >= 0; i--)
        {
            var rb = activeSpikes[i];
            if (rb == null) { activeSpikes.RemoveAt(i); continue; }

            // Ensure sensible physics settings for chase motion
            rb.gravityScale = 0f; // pure horizontal glide
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            float dirSign = Mathf.Sign(player.position.x - rb.position.x);
            if (dirSign == 0f) dirSign = 1f; // arbitrary, if exactly aligned

            // Stop if a wall is directly ahead
            Vector2 dir = new Vector2(dirSign, 0f);
            RaycastHit2D hit = Physics2D.Raycast(rb.position, dir, wallCheckDistance, wallLayer);
            Debug.DrawLine(rb.position, rb.position + dir * wallCheckDistance, hit ? Color.red : Color.green);

            if (hit.collider != null)
            {
                rb.linearVelocity = Vector2.zero;
                // Optionally remove from active so it no longer moves
                // activeSpikes.RemoveAt(i);
                continue;
            }

            // Apply horizontal chase velocity (keep Y unchanged)
            rb.linearVelocity = new Vector2(dirSign * chaseSpeed, rb.linearVelocity.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize find radius + trigger shape
        Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
        Gizmos.DrawWireSphere(transform.position, findRadius);

        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(1f, 0.4f, 0.4f, 0.35f);
            var b = col.bounds;
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
}
