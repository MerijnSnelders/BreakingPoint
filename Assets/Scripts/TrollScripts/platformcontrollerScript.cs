using System;
using System.Collections;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 1.5f;
    public float playerSpeedMultiplier = 2f;
    public float bottomY = -6f;
    public float topY = 6f;
    public float verticalSpacing = 1.5f; // spacing between platforms when respawning

    private bool playerOnPlatform = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
    }

    void FixedUpdate()
    {
        // Only move down when not dynamic (normal mode)
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Debug.Log("here");
            float moveSpeed = playerOnPlatform ? speed * playerSpeedMultiplier : speed; // Move with physics system (prevents jitter)
            Vector2 newPos = rb.position + Vector2.down * speed * Time.fixedDeltaTime; rb.MovePosition(newPos); // Respawn check
            if (rb.position.y <= bottomY) 
            {
                Debug.Log("here1");
                ResetPlatformAfterDelay();
                //    rb.position = new Vector2(rb.position.x, topY); 
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = true;

            // Make the platform dynamic so it falls naturally
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 5f;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;
            ResetPlatformAfterDelay();
        }
    }

    private void ResetPlatformAfterDelay()
    {
        // Wait before resetting so it falls a bit
       // yield return new WaitForSeconds(1.5f);

        // Reset to normal
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Respawn above if below bottom
        if (rb.position.y <= bottomY)
        {
            Debug.Log("here3");
            float newY = topY + 0f + verticalSpacing;
            rb.position = new Vector2(rb.position.x, newY);
        }
    }
}
