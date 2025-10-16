using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;
    private SpriteRenderer sr;

    [Header("Colors")]
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.green;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = inactiveColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        if (collision.CompareTag("Player"))
        {
            activated = true;
            if (sr != null)
                sr.color = activeColor;

            // Get the PlayerDeath component and set checkpoint
            PlayerDeath playerDeath = collision.GetComponent<PlayerDeath>();
            if (playerDeath != null)
            {
                playerDeath.SetCheckpoint(transform.position);
            }
        }
    }
}
