using UnityEngine;
using System.Collections;

public class SplitPlatform : MonoBehaviour
{
    [Header("Platform Parts")]
    [SerializeField] private Rigidbody2D leftPart;
    [SerializeField] private Rigidbody2D rightPart;

    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float moveDistance = 1.5f; // how far halves move
    [SerializeField] private float moveSpeed = 3f;      // how fast they slide apart

    private bool triggered = false;
    private Vector2 leftStartPos;
    private Vector2 rightStartPos;

    private void Start()
    {
        // remember starting positions for smooth movement
        leftStartPos = leftPart.position;
        rightStartPos = rightPart.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag(playerTag)) return;

        triggered = true;
        StartCoroutine(SplitApart());
    }

    private IEnumerator SplitApart()
    {
        Vector2 leftTarget = leftStartPos + Vector2.left * moveDistance;
        Vector2 rightTarget = rightStartPos + Vector2.right * moveDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            leftPart.MovePosition(Vector2.Lerp(leftStartPos, leftTarget, t));
            rightPart.MovePosition(Vector2.Lerp(rightStartPos, rightTarget, t));
            yield return null;
        }
    }
}