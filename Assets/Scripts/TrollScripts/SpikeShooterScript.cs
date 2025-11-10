using System.Collections;
using UnityEngine;

public class SpikeShooterScript : MonoBehaviour
{
    [Header("Spike Settings")]
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float spikeSpeed = 10f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float destroySpikeTimer = 5f;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private float playDuration = 0.5f; // how long to play from the clip
    [SerializeField] private bool randomizePitch = true;

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
            Debug.LogWarning($"⚠️ Missing spikePrefab or shootPoint on {gameObject.name}");
            return;
        }

        // 🎧 Play sound instantly (no coroutine, no delay)
        if (audioSource != null && shootClip != null)
        {
            if (randomizePitch)
                audioSource.pitch = Random.Range(0.95f, 1.05f);

            // Play only the first short section of the clip
            audioSource.clip = shootClip;
            audioSource.time = 0f;
            audioSource.Play();

            // stop it shortly after to only play the "burst" part
            Invoke(nameof(StopAudioEarly), playDuration);
        }

        // Instantiate and launch spike
        Quaternion leftRotation = Quaternion.Euler(0, 0, 90);
        GameObject newSpike = Instantiate(spikePrefab, shootPoint.position, leftRotation);

        Rigidbody2D rb = newSpike.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.left * spikeSpeed;

        Destroy(newSpike, destroySpikeTimer);
    }

    // Stops the sound after the short section
    private void StopAudioEarly()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    private IEnumerator PlayPartialSound()
    {
        audioSource.clip = shootClip;
        audioSource.time = 0f; // start from beginning
        audioSource.pitch = randomizePitch ? Random.Range(0.95f, 1.05f) : 1f;

        audioSource.Play();
        yield return new WaitForSeconds(playDuration); // only play first part
        audioSource.Stop();

        audioSource.pitch = 1f;
    }
}
