using UnityEngine;

public class RandomFallingSPikeTriggerScript : MonoBehaviour
{
    public RandomFallingSpikeScript spikeManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spikeManager.StartFalling();
        }
    }
}
