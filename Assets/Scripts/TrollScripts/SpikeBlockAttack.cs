using System.Collections;
using UnityEngine;

public class SpikeBlockAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform player;

    [Header("Attack Settings")]
    [SerializeField] private float attackForce = 25f;     // Higher = faster lunge
    [SerializeField] private float delayAfterExit = 0.2f; // Wait before lunging
    [SerializeField] private bool oneTime = true;
    [SerializeField] private float tiltAngle = 45f;       // Desired rotation angle
    [SerializeField] private float tiltSpeed = 200f;      // How fast it tilts

    private bool hasAttacked = false;
    private bool playerInside = false;
    private int deadlyLayer;

    private void Awake()
    {
        deadlyLayer = LayerMask.NameToLayer("DeadlyObject");
        if (deadlyLayer == -1)
        {
            Debug.LogWarning("Layer 'DeadlyObject' not found! Please create it in Edit > Project Settings > Tags and Layers.");
        }
        else
        {
            gameObject.layer = deadlyLayer;
        }
    }

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true; // we handle rotation manually now
        }
    }

    // ✅ Called from TriggerForwarder
    public void PlayerEntered()
    {
        playerInside = true;
    }

    // ✅ Called from TriggerForwarder
    public void PlayerExited()
    {
        if (!playerInside) return;
        playerInside = false;

        if (!hasAttacked || !oneTime)
            Invoke(nameof(AttackPlayer), delayAfterExit);
    }

    private void AttackPlayer()
    {
        if (hasAttacked && oneTime) return;
        hasAttacked = true;

        if (rb == null || player == null) return;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * attackForce; // 💨 direct velocity = instant fast lunge

        // Slight tilt for visual punch
        float finalAngle = transform.rotation.eulerAngles.z + tiltAngle * Mathf.Sign(dir.x);
        StartCoroutine(RotateSmoothly(finalAngle));
    }

    // Smoothly tilts the block by ~45°
    private IEnumerator RotateSmoothly(float targetZ)
    {
        float startZ = transform.rotation.eulerAngles.z;
        float elapsed = 0f;

        while (elapsed < Mathf.Abs(targetZ - startZ) / tiltSpeed)
        {
            elapsed += Time.deltaTime;
            float newZ = Mathf.LerpAngle(startZ, targetZ, elapsed * tiltSpeed / Mathf.Abs(targetZ - startZ));
            transform.rotation = Quaternion.Euler(0, 0, newZ);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, targetZ);
    }
}
