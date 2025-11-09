using UnityEngine;

public class FallingBoulder : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask extraLayers; // Additional layers you can select in the Inspector
    [SerializeField] private float fallSpeed = 8f;
    [SerializeField] private float smashWidth = 1.5f;
    [SerializeField] private float smashDepth = 1.0f;
    [SerializeField] private GameObject destroyEffectPrefab;

    [Header("Optional - Objects that should fall when ground breaks")]
    [SerializeField] private Rigidbody2D[] objectsToFall;

    private Rigidbody2D rb;
    private LayerMask combinedLayerMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;

        // Ensure the “Ground” layer is always included
        int groundLayer = LayerMask.NameToLayer("Ground");
        combinedLayerMask = extraLayers | (1 << groundLayer);
    }

    public void Drop() => rb.gravityScale = fallSpeed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsInCombinedLayer(collision.gameObject.layer))
        {
            MakeAssignedObjectsFall();
            SmashGroundInstantly();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsInCombinedLayer(collision.gameObject.layer))
        {
            SmashGroundInstantly();
        }
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

            // ✅ Detect if player is nearby
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                playerNearby = true;
                continue; // Don’t destroy player here, just mark it
            }

            // ❌ Skip destroying the boulder itself
            if (hit.gameObject == gameObject)
                continue;

            // ✅ Destroy other ground objects
            if (destroyEffectPrefab != null)
            {
                GameObject fx = Instantiate(destroyEffectPrefab, hit.transform.position, Quaternion.identity);
                Destroy(fx, 1f);
            }

            Destroy(hit.gameObject);
        }

        // 💀 If player was on or right above destroyed ground, kill him
        if (playerNearby)
        {
            PlayerDeath player = FindObjectOfType<PlayerDeath>();
            if (player != null)
            {
                Debug.Log("💀 Player killed by collapsing ground.");
                player.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            }
        }

        // Stop boulder motion after smash
        rb.gravityScale = fallSpeed;
        rb.constraints = RigidbodyConstraints2D.None; // allow continued fall
        rb.freezeRotation = true; // still prevent tilting

        Debug.Log($"💥 Boulder destroyed {hits.Length} ground colliders and checked for player.");
    }




    private void MakeAssignedObjectsFall()
    {
        foreach (Rigidbody2D obj in objectsToFall)
        {
            if (obj != null)
                obj.bodyType = RigidbodyType2D.Dynamic;
        }
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
