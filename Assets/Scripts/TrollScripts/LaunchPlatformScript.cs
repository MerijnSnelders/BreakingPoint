using UnityEngine;

public class LaunchPlatformScript : MonoBehaviour
{
    [Header("Launch Settings")]
    public float launchSpeed = 15f;   // horizontal speed
    public bool pushRight = false;    // choose direction
    public float launchDuration = 0.3f; // how long input is disabled

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        var player = collision.gameObject.GetComponent<PlayerMovementScript>();
        if (player != null)
        {
            Vector2 launchVelocity = new Vector2(pushRight ? launchSpeed : -launchSpeed, 0f);
            player.Launch(launchVelocity, launchDuration);
            Debug.Log($"Launched player with velocity {launchVelocity}");
        }
    }
}