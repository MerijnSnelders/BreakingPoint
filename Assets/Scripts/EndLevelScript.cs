using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelScript : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Name of the scene to load when the player touches the door.")]
    public string nextSceneName;

    [Tooltip("Optional delay before switching scenes (e.g., for animations).")]
    public float transitionDelay = 0.5f;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTransitioning) return;

        if (collision.CompareTag("Player"))
        {
            isTransitioning = true;
            Invoke(nameof(LoadNextScene), transitionDelay);
        }
    }

    private void LoadNextScene()
    {
        ResetCheckpointData();

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("⚠️ No scene name set on " + gameObject.name);
        }
    }

    private void ResetCheckpointData()
    {
        // Access static fields in PlayerDeath and reset them
        var type = typeof(PlayerDeath);
        type.GetField("hasCheckpoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.SetValue(null, false);
        type.GetField("diedOnce", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.SetValue(null, false);
    }
}

