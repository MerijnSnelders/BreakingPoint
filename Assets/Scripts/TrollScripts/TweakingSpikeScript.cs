using System.Collections;
using UnityEngine;

public class TweakingSpikeScript : MonoBehaviour
{
    [Header("Spikes to Move")]
    [SerializeField] private GameObject[] spikes; // One or multiple spikes

    [Header("Movement Settings")]
    [SerializeField] private Vector2 moveDirection = Vector2.up; // Direction to move (up, right, etc.)
    [SerializeField] private float moveDistance = 0.5f;          // How far the spikes move
    [SerializeField] private float moveSpeed = 3f;               // How fast they move
    [SerializeField] private float stayUpTime = 0.5f;            // Time spikes stay up before going down
    [SerializeField] private bool oneTime = true;                // Should it trigger only once?

    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (oneTime && isActivated) return;

        if (other.CompareTag("Player"))
        {
            isActivated = true;

            foreach (GameObject spike in spikes)
            {
                if (spike != null)
                    StartCoroutine(MoveSpike(spike.transform));
            }

            // Allow retrigger if not oneTime
            if (!oneTime)
                StartCoroutine(ResetActivation());
        }
    }

    private IEnumerator MoveSpike(Transform spikeTransform)
    {
        Vector3 startPos = spikeTransform.position;
        Vector3 targetPos = startPos + (Vector3)(moveDirection.normalized * moveDistance);

        // Move up
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

        // Stay up for a moment
        yield return new WaitForSeconds(stayUpTime);

        // Move back down
        while (Vector3.Distance(spikeTransform.position, startPos) > 0.01f)
        {
            spikeTransform.position = Vector3.MoveTowards(
                spikeTransform.position,
                startPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        spikeTransform.position = startPos;
    }

    private IEnumerator ResetActivation()
    {
        // Short delay before reactivation allowed
        yield return new WaitForSeconds(stayUpTime + 1f);
        isActivated = false;
    }
}
