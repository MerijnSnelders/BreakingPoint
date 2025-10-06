using System.Collections;
using UnityEngine;

public class FallingPlatformWithWalls : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Force Walls")]
    public Collider2D leftWall;
    public Collider2D rightWall;
    public float wallDuration = 1f; // how long walls stay active

    [Header("Fall Settings")]
    public float fallGravity = 5f;

    [Header("Trigger")]
    public Collider2D activationTrigger; // assign the trigger collider here

    private bool hasFallen = false; // prevent multiple activations

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // ensure walls are off at start
        if (leftWall != null) leftWall.enabled = false;
        if (rightWall != null) rightWall.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasFallen) return;

        if (other.CompareTag("Player")) // player touched the trigger
        {
            hasFallen = true;
            FallAndActivateWalls();
        }
    }


    private void FallAndActivateWalls()
    {
        // Make the platform fall
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;

        // Activate walls temporarily
        StartCoroutine(ActivateWalls());
    }

    private IEnumerator ActivateWalls()
    {
        // wait one physics frame to ensure collisions are detected
        yield return new WaitForFixedUpdate();

        if (leftWall != null) leftWall.enabled = true;
        if (rightWall != null) rightWall.enabled = true;

        yield return new WaitForSeconds(wallDuration);

        if (leftWall != null) leftWall.enabled = false;
        if (rightWall != null) rightWall.enabled = false;
    }
}