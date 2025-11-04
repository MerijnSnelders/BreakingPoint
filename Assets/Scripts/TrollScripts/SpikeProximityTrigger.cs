using UnityEngine;

public class FollowingSpikesTriggerScript : MonoBehaviour
{
    public SpikesFollowingPlayer spike;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            spike.TriggerRise();
    }
}
