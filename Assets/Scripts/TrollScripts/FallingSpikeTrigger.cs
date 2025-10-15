using UnityEngine;

/// <summary>
/// Triggers an array of FallingSpike objects to begin falling when the player enters this area.
/// </summary>
public class FallingSpikesTrigger : MonoBehaviour
{
    [Header("Spikes to trigger")]
    // Drag all spike GameObjects with the FallingSpike script here
    public FallingSpike[] spikes;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only trigger once
        if (triggered) return;

        // Check for the Player tag
        if (other.CompareTag("Player"))
        {
            triggered = true;
            // Iterate through all linked spikes and tell them to start falling
            foreach (var spike in spikes)
                spike.TriggerFall();
        }
    }
}