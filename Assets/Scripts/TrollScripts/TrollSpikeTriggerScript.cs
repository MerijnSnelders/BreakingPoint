using UnityEngine;

public class TrollSpikeTriggerScript : MonoBehaviour
{
    public TrollSpikeScript spike;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger");
        if (other.CompareTag("Player"))
        {
            Debug.Log("trigger2");
            spike.ActivateSpike();
            // Optional: disable the trigger so it only activates once
            gameObject.SetActive(false);
        }
    }
}
