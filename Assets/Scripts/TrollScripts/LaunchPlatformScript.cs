using UnityEngine;

public class LaunchPlatformScript : MonoBehaviour
{
    [Header("Launch Settings")]
    public float launchSpeed = 15f;       // horizontal speed
    public bool pushRight = false;        // choose direction
    public float launchDuration = 0.3f;   // how long input is disabled
    [Range(0f, 1f)]
    public float topTolerance = 0.5f;     // how "flat" the top hit must be

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // check if player is colliding from above
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // the normal points *outward* from the platform surface
            // so a contact normal pointing upward means the player is above the platform
            if (contact.normal.y < -topTolerance) // e.g., -0.5 means player is landing on top
            {
                var player = collision.gameObject.GetComponent<PlayerMovementScript>();
                if (player != null)
                {
                    Vector2 launchVelocity = new Vector2(pushRight ? launchSpeed : -launchSpeed, 0f);
                    player.Launch(launchVelocity, launchDuration);
                    Debug.Log($"Launched player with velocity {launchVelocity}");
                }

                return; // launch once and stop
            }
        }
    }
}