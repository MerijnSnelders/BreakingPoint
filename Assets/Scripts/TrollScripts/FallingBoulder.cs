using UnityEngine;

public class FallingBoulder : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallSpeed = 8f;
    [SerializeField] private float smashWidth = 1.5f;
    [SerializeField] private float smashDepth = 1.0f;
    [SerializeField] private GameObject destroyEffectPrefab;

    [Header("Optional - Objects that should fall when ground breaks")]
    [SerializeField] private Rigidbody2D[] objectsToFall;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    // Called by your trigger to make it fall
    public void Drop() => rb.gravityScale = fallSpeed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            MakeAssignedObjectsFall();
            SmashGroundInstantly();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            SmashGroundInstantly();
        }
    }

    private void SmashGroundInstantly()
    {
        // Detect boulder size
        float boulderHeight = 1f;
        Collider2D boulderCollider = GetComponent<Collider2D>();
        if (boulderCollider != null)
            boulderHeight = boulderCollider.bounds.size.y;

        // Place destruction box right below the boulder
        float offset = boulderHeight / 2f + smashDepth / 2f;
        Vector2 boxCenter = new Vector2(transform.position.x, transform.position.y - offset);
        Vector2 boxSize = new Vector2(smashWidth, smashDepth);

        // Find ground colliders under it
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f, groundLayer);

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            // 💥 Spawn effect at the destroyed block's position
            if (destroyEffectPrefab != null)
            {
                Instantiate(
                    destroyEffectPrefab,
                    hit.transform.position,
                    Quaternion.identity
                );
                Destroy(destroyEffectPrefab, 1f);
            }

            Destroy(hit.gameObject);
        }

        // Slight nudge downward to ensure continued fall
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -5f);

        Debug.Log($"Destroyed {hits.Length} ground colliders under boulder.");
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
