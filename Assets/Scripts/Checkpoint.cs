using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visuals")]
    public SpriteRenderer flagRenderer;      // Assign your flag sprite
    public Animator flagAnimator;            // Optional – assign if you have animation


    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Player"))
        {
            // ✅ Update player checkpoint
            PlayerDeath playerDeath = collision.GetComponent<PlayerDeath>();
            if (playerDeath != null)
                playerDeath.SetCheckpoint(transform.position);

            ActivateFlag();
        }
    }

    private void ActivateFlag()
    {
        // Trigger animation if available
        if (flagAnimator != null)
            flagAnimator.SetTrigger("Activate");

        Debug.Log("Checkpoint activated!");
    }
}
