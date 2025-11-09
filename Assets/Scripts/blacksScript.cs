using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("The platform prefab to be spawned. Drag your platform prefab here.")]
    public GameObject platformPrefab;

    [Tooltip("The time in seconds between each platform spawn.")]
    public float spawnRate = 3.0f;

    [Tooltip("The exact position where new platforms will be spawned.")]
    public Vector3 spawnPosition;

    void Start()
    {
        if (platformPrefab == null)
        {
            Debug.LogError("Platform Prefab is not set on the Spawner!", this);
            return;
        }

        // Call the SpawnPlatform function every 'spawnRate' seconds, starting after 0 seconds.
        InvokeRepeating("SpawnPlatform", 0f, spawnRate);
    }

    /// <summary>
    /// Instantiates a new platform at the spawn position.
    /// </summary>
    void SpawnPlatform()
    {
        // Create a new instance of the platform prefab at the specified position
        Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
    }
}
