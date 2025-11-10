using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;
    private AudioSource audioSource;

    [Header("Scenes where this music should play")]
    [Tooltip("Add the names of the scenes where the music should continue playing.")]
    [SerializeField] private string[] allowedScenes;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the new scene is allowed
        bool isAllowed = false;
        foreach (string allowed in allowedScenes)
        {
            if (scene.name == allowed)
            {
                isAllowed = true;
                break;
            }
        }

        // If not allowed → stop and destroy the object
        if (!isAllowed)
        {
            audioSource.Stop();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }
}