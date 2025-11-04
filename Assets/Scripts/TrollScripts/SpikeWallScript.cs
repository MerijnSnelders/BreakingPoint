using System.Collections;
using UnityEngine;

public class SpikeWallScript : MonoBehaviour
{
    [Header("Wall Settings")]
    [SerializeField] private GameObject wall;

    [Header("Rise Settings")]
    [SerializeField] private Vector2 riseDirection = Vector2.up;
    [SerializeField] private float riseDistance = 1f;
    [SerializeField] private float riseSpeed = 3f;

    [Header("Fall Settings")]
    [SerializeField] private float delayBeforeFall = 1.5f;
    [SerializeField] private float sidePush = 2f;      // small horizontal nudge
    [SerializeField] private float tipTorque = 5f;     // gentle tipping force
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float angularDrag = 2f;   // slows rotation naturally

    [Header("Other Settings")]
    [SerializeField] private bool oneTime = true;

    private bool isActivated = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = wall.GetComponent<Rigidbody2D>();

        // Setup Rigidbody defaults
        rb.gravityScale = 0;
        rb.angularDamping = angularDrag;

        // Keep it frozen at the start (so it doesn’t drift)
        rb.constraints = RigidbodyConstraints2D.FreezePositionX |
                         RigidbodyConstraints2D.FreezePositionY |
                         RigidbodyConstraints2D.FreezeRotation;


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (oneTime && isActivated) return;

        if (other.CompareTag("Player"))
        {
            isActivated = true;
            StartCoroutine(RiseThenTipOver());
        }
    }

    private IEnumerator RiseThenTipOver()
    {
        // 1️⃣ Disable physics while rising
        rb.isKinematic = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        Vector3 startPos = wall.transform.position;
        Vector3 targetPos = startPos + (Vector3)(riseDirection.normalized * riseDistance);

        // 2️⃣ Rise up smoothly
        while (Vector3.Distance(wall.transform.position, targetPos) > 0.01f)
        {
            wall.transform.position = Vector3.MoveTowards(
                wall.transform.position,
                targetPos,
                riseSpeed * Time.deltaTime
            );
            yield return null;
        }

        wall.transform.position = targetPos;

        // 3️⃣ Wait before falling
        yield return new WaitForSeconds(delayBeforeFall);

        // 4️⃣ Disable spike colliders (so they don’t get stuck in ground)
        DisableSpikeColliders();

        // 5️⃣ Reactivate physics and allow falling
        rb.isKinematic = false;
        rb.gravityScale = gravityScale;

        // Unfreeze all constraints so it can fall and rotate
        rb.constraints = RigidbodyConstraints2D.None;

        // 6️⃣ Apply a small left push and gentle torque to tip it over
        rb.AddForce(Vector2.left * sidePush, ForceMode2D.Impulse);
        rb.AddTorque(tipTorque, ForceMode2D.Impulse);
    }

    private void DisableSpikeColliders()
    {
        Collider2D[] colliders = wall.GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            // Disable all child colliders except the main one on the wall
            if (col.gameObject != wall)
                col.enabled = false;
        }
    }
}
