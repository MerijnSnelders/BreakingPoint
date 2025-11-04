using UnityEngine;

public class FallLevel2Trigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDeath deathScript = other.GetComponent<PlayerDeath>();
            if (deathScript != null)
            {
                // ✅ Disable fall-death permanently
                deathScript.isLevel3 = true;
                Debug.Log("☠️ Fall trap triggered — fall death disabled permanently.");

                // Optional: give the player a little downward nudge
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, -10f);
            }
        }
    }
}
