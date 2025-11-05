using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("⚠️ Player not found in scene!");
            return;
        }

        // 🟩 Get PlayerDeath component
        PlayerDeath deathScript = player.GetComponent<PlayerDeath>();

        // 🟩 If player died once and checkpoint exists, use it
        if (deathScript != null && HasCheckpoint())
        {
            player.transform.position = GetCheckpointPosition();
        }
        else
        {
            player.transform.position = transform.position;
        }
    }

    // ✅ Helpers to access PlayerDeath static data
    private bool HasCheckpoint()
    {
        var field = typeof(PlayerDeath).GetField("hasCheckpoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        return field != null && (bool)field.GetValue(null);
    }

    private Vector2 GetCheckpointPosition()
    {
        var field = typeof(PlayerDeath).GetField("respawnPoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        return field != null ? (Vector2)field.GetValue(null) : Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
