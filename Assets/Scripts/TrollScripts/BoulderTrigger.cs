using UnityEngine;
using System.Collections;

public class BoulderTrigger : MonoBehaviour
{
    [SerializeField] private FallingBoulder boulder; // assign in Inspector
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float delayBeforeFall = 0f; // ✅ delay in seconds, default 0

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag(playerTag)) return;

        triggered = true;
        StartCoroutine(TriggerFallAfterDelay());
    }

    private IEnumerator TriggerFallAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeFall);
        if (boulder != null)
            boulder.Drop();

        Destroy(gameObject); // optional: remove trigger after use
    }
}