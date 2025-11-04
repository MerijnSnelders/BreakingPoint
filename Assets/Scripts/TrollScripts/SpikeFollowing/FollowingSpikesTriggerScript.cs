using UnityEngine;

public class SpikeProximityTrigger : MonoBehaviour
{
    public SpikesFollowingPlayer spike;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            spike.TriggerRise();
    }
}
