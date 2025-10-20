using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visuals")]
    public SpriteRenderer flagRenderer;
    public Animator flagAnimator;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip checkpointSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerDeath playerDeath = collision.GetComponent<PlayerDeath>();
        if (playerDeath != null)
            playerDeath.SetCheckpoint(transform.position);

        ActivateFlag();

        // ✅ Only play once per level
        if (!PlayerDeath.HasCheckpointSoundPlayed())
        {
            PlayCheckpointSound();
            PlayerDeath.MarkCheckpointSoundPlayed();
        }
    }

    private void ActivateFlag()
    {
        if (flagAnimator != null)
            flagAnimator.SetTrigger("Activate");
    }

    private void PlayCheckpointSound()
    {
        if (audioSource != null && checkpointSound != null)
            audioSource.PlayOneShot(checkpointSound);
    }
}