using UnityEngine;

public class SpikeChaseTrigger : MonoBehaviour
{
    public SpikesFollowingPlayer spike;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[{name}] Player ENTERED chase trigger");
            spike.StartChasing();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[{name}] Player EXITED chase trigger");
            spike.StopChasing();
        }
    }
}