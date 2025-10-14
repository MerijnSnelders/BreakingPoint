using System.Collections;
using UnityEngine;

public class FallingGround_Delayed : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isTriggered = false;

    [Header("Settings")]
    public float fallGravity = 5f;       // how fast it falls
    public float delayBeforeFall = 0.5f; // seconds before it drops

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
}
