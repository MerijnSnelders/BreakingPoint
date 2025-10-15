using UnityEngine;

public class FollowingSpikesTriggerScript : MonoBehaviour
{
    [Header("Spikes to trigger")]
    public SpikesFollowingPlayer[] spikes; // drag all spikes here in Inspector
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            triggered = true;
            foreach (var spike in spikes)
                spike.TriggerRise();
        }
    }
}
