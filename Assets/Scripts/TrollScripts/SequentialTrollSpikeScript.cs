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
    [Tooltip("Time delay (in seconds) before the entire sequence begins.")]
    [SerializeField] private float initialStartDelay = 0.5f;
    [Tooltip("Time delay (in seconds) between triggering each spike's movement.")]
    [SerializeField] private float delayBetweenSpikes = 0.05f;

    [Header("Optional Audio")]
    [SerializeField] private AudioSource audioSource; // Optional shared audio source
    [SerializeField] private AudioClip spikePopClip;  // Optional spike sound
    [SerializeField, Tooltip("If true, each spike gets a slightly different pitch")]
    private bool randomizePitch = true;

    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (oneTime && isActivated) return;

        if (other.CompareTag("Player"))
        {
            isActivated = true;
            StartCoroutine(TriggerSpikesSequentially());
        }
    }

    private IEnumerator TriggerSpikesSequentially()
    {
        // Wait before the whole sequence starts
        if (initialStartDelay > 0)
            yield return new WaitForSeconds(initialStartDelay);

        // Trigger spikes one by one
        foreach (GameObject spike in spikes)
        {
            if (spike != null)
            {
                // 🎧 Play sound immediately when this spike activates
                if (audioSource != null && spikePopClip != null)
                {
                    if (randomizePitch)
                        audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(spikePopClip);
                }

                // 🗡️ Move the spike
                StartCoroutine(MoveSpike(spike.transform));
            }

            // Wait before triggering the next spike
            yield return new WaitForSeconds(delayBetweenSpikes);
        }
    }

    private IEnumerator MoveSpike(Transform spikeTransform)
    {
        Vector3 startPos = spikeTransform.position;
        Vector3 targetPos = startPos + (Vector3)(moveDirection.normalized * moveDistance);

        while (Vector3.Distance(spikeTransform.position, targetPos) > 0.01f)
        {
            spikeTransform.position = Vector3.MoveTowards(
                spikeTransform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        spikeTransform.position = targetPos;
    }
}
