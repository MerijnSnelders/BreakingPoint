using UnityEngine;

public class SpikeShooterScript : MonoBehaviour
{
    [Header("Spike Settings")]
    public GameObject spikePrefab;      // Assign your spike prefab here
    public Transform shootPoint;        // Where spikes are spawned from
    public float spikeSpeed = 10f;      // How fast the spikes move
    public float fireRate = 1f;         // Time (seconds) between shots
    public float destroySpikeTimer = 5f;
    private float fireTimer = 0f;
    

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            ShootSpike();
            fireTimer = 0f;
        }
    }

    private void ShootSpike()
    {
        if (spikePrefab == null || shootPoint == null)
        {
            Debug.LogWarning("⚠️ Missing spikePrefab or shootPoint on " + gameObject.name);
            return;
        }


        Quaternion leftRotation = Quaternion.Euler(0, 0, 90);
        // Instantiate a new spike
        GameObject newSpike = Instantiate(spikePrefab, shootPoint.position, leftRotation);

        // Apply velocity to move it to the left
        Rigidbody2D rb = newSpike.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.left * spikeSpeed;
        }

        // Optionally destroy after a few seconds to avoid clutter
        Destroy(newSpike, destroySpikeTimer);
    }
}
