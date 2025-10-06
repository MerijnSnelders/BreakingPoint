using UnityEngine;

public class TrollSpikeScript : MonoBehaviour
{
    public Transform targetPosition; // Where the spike should slide to
    public float slideSpeed = 5f;

    private bool activated = false;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (activated)
        {
            // Move towards target position smoothly
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, slideSpeed * Time.deltaTime);
        }
    }

    // Call this when player hits the trigger
    public void ActivateSpike()
    {
        activated = true;
    }
}
