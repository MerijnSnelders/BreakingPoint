using System.Collections;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private float chargeSpeed = 8f;      // Speed of the charge
    [SerializeField] private float chargeDelay = 0.5f;    // Delay before charge starts
    [SerializeField] private string playerTag = "Player"; // Tag of the player
    [SerializeField] private LayerMask wallLayer;         // What stops the spike

    private Rigidbody2D rb;
    private bool isCharging = false;
    private bool hasCharged = false; // prevents multiple charges

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasCharged && collision.CompareTag(playerTag))
        {
            StartCoroutine(ChargeAfterDelay(collision.transform));
        }
    }

    private IEnumerator ChargeAfterDelay(Transform player)
    {
        hasCharged = true;
        yield return new WaitForSeconds(chargeDelay);

        // Determine charge direction
        Vector2 direction = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;

        isCharging = true;
        rb.linearVelocity = direction * chargeSpeed;
    }

    private void Update()
    {
        if (!isCharging) return;

        // Check if wall is in front of the spike
        Vector2 dir = Mathf.Sign(rb.linearVelocity.x) * Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.5f, wallLayer);

        if (hit.collider != null)
        {
            StopCharge();
        }
    }

    private void StopCharge()
    {
        rb.linearVelocity = Vector2.zero;
        isCharging = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * 0.5f);
    }
}
