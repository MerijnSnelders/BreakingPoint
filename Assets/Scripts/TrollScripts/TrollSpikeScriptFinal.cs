using System.Collections;
using UnityEngine;

public class TrollSpikeScriptFinal : MonoBehaviour
{
    [Header("Spikes to Move")]
    [SerializeField] private GameObject[] spikes; // One or multiple spikes

    [Header("Movement Settings")]
    [SerializeField] private Vector2 moveDirection = Vector2.up; // Direction to move (e.g., up, right)
    [SerializeField] private float moveDistance = 0.5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool oneTime = true; // Only trigger once or reusable

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
