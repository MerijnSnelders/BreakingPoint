using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelMusic : MonoBehaviour
{
    private static LevelMusic instance;
    private AudioSource audioSource;
    private string currentSceneName;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 2.5f;
    [SerializeField] private float baseVolume = 0.5f;

    private bool isFadingOut = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            currentSceneName = SceneManager.GetActiveScene().name;

            audioSource.volume = baseVolume;
            audioSource.Play();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If same scene was reloaded → fade in again
        if (scene.name == currentSceneName)
        {
            StartCoroutine(FadeIn());
        }
        else
        {
            // Different scene → destroy the music object
            audioSource.Stop();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }

    public IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, baseVolume, elapsed / fadeInDuration);
            yield return null;
        }
        audioSource.volume = baseVolume;
    }

    public IEnumerator FadeOut()
    {
        if (isFadingOut) yield break;
        isFadingOut = true;

        float startVolume = audioSource.volume;
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        isFadingOut = false;
    }
}
