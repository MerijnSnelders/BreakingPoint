using System.Collections;
using UnityEngine;

public class SequentialTrollSpikeScript : MonoBehaviour
{
    [Header("Spikes to Move")]
    [SerializeField] private GameObject[] spikes; // One or multiple spikes

    [Header("Movement Settings")]
    [SerializeField] private Vector2 moveDirection = Vector2.up; // Direction to move (e.g., up, right)
    [SerializeField] private float moveDistance = 0.5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool oneTime = true; // Only trigger once or reusable

    [Header("Sequential Settings")]
    [Tooltip("The time delay (in seconds) before the *entire sequence* begins.")]
    [SerializeField] private float initialStartDelay = 0.5f; // New variable for the initial delay
    [Tooltip("The time delay (in seconds) between triggering each spike's movement.")]
    [SerializeField] private float delayBetweenSpikes = 0.05f; // Fast default delay

    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (oneTime && isActivated) return;

        if (other.CompareTag("Player"))
        {
            isActivated = true;
            // Start the new coroutine to trigger spikes one by one
            StartCoroutine(TriggerSpikesSequentially());
        }
    }

    /// <summary>
    /// Coroutine to iterate through the spikes and trigger their movement one by one with a delay.
    /// </summary>
    private IEnumerator TriggerSpikesSequentially()
    {
        // New: Wait for the initial delay before starting the spike movement sequence
        if (initialStartDelay > 0)
        {
            yield return new WaitForSeconds(initialStartDelay);
        }

        foreach (GameObject spike in spikes)
        {
            if (spike != null)
            {
                // Start the movement Coroutine for the current spike
                StartCoroutine(MoveSpike(spike.transform));
            }
            // Wait for the specified delay before starting the next spike's movement
            yield return new WaitForSeconds(delayBetweenSpikes);
        }
    }

    /// <summary>
    /// Handles the smooth movement of a single spike to its target position.
    /// </summary>
    private IEnumerator MoveSpike(Transform spikeTransform)
    {
        Vector3 startPos = spikeTransform.position;
        // Calculate the target position by adding the normalized direction scaled by distance
        Vector3 targetPos = startPos + (Vector3)(moveDirection.normalized * moveDistance);

        while (Vector3.Distance(spikeTransform.position, targetPos) > 0.01f)
        {
            spikeTransform.position = Vector3.MoveTowards(
                spikeTransform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null; // Wait for the next frame
        }

        // Snap to the exact target position to prevent floating-point errors
        spikeTransform.position = targetPos;
    }
}
