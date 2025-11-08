using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject platformPrefab;
    public int platformCount = 8;
    public float startY = 6f;
    public float verticalSpacing = 1.5f;
    public float startX = 2f;
    public float bottemY = -10f;

    [Header("Platform Speed Settings")]
    public float baseSpeed = 1.5f;
    public float speedRandomness = 0.3f;

    void Start()
    {
        if (!platformPrefab)
        {
            Debug.LogError("PlatformSpawner: no platformPrefab assigned!");
            return;
        }

        for (int i = 0; i < platformCount; i++)
        {
            //float randomSpeed = baseSpeed + Random.Range(-speedRandomness, speedRandomness);

            // Keep spacing consistent
            Vector3 spawnPos = new Vector3(startX, startY - (i * verticalSpacing), 0f);
            GameObject p = Instantiate(platformPrefab, spawnPos, Quaternion.identity);

            PlatformController pc = p.GetComponent<PlatformController>();
            if (pc != null)
            {
                pc.speed = baseSpeed;
                pc.topY = startY;
                pc.bottomY = bottemY;
                pc.verticalSpacing = verticalSpacing;
            }
        }
    }
}
