using System.Collections;
using UnityEngine;


public class InvisibleBlockShowingUp : MonoBehaviour
{
    private Collider2D blockCollider;
    private SpriteRenderer blockRenderer;
    private bool isActive = false;

    [Header("Pop-Up Settings")]
    public float popHeight = 0.2f;
    public float popSpeed = 5f;

    [Header("Trigger")]
    public Collider2D bottomTrigger; // assign this in Inspector

    void Awake()
    {
        blockCollider = GetComponent<Collider2D>();
        blockRenderer = GetComponent<SpriteRenderer>();

        blockCollider.enabled = false;
        blockRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive) return;
        if (!other.CompareTag("Player")) return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        // Only trigger if the player is moving upwards (jumping)
        if (playerRb.linearVelocity.y > 0f)
        {
            ActivateBlock();
        }
    }


    private void ActivateBlock()
    {
        isActive = true;

        blockCollider.enabled = true;
        blockRenderer.enabled = true;

        StartCoroutine(PopUpAnimation());
    }

    private IEnumerator PopUpAnimation()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * popHeight;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * popSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.Sin(t * Mathf.PI * 0.5f));
            yield return null;
        }

        transform.position = endPos;
    }

}

