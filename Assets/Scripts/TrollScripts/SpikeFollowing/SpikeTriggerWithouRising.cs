using UnityEngine;

public class SpikeTriggerWithouRising : MonoBehaviour
{
    public SpikesFollowingPlayerWithoutRising spike;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            spike.StartChasing();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            spike.StopChasing();
    }
}
