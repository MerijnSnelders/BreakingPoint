using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelMusic : MonoBehaviour
{
    private static LevelMusic instance;
    private AudioSource audioSource;

    [Header("Music Per Scene")]
    [SerializeField] private SceneMusic[] sceneMusicList;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float baseVolume = 0.5f;

    private Dictionary<string, AudioClip> musicLookup;
    private string currentScene;

    // NEW — resume system
    private float savedTime = 0f;
    private bool resumeAfterReload = false;

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip musicClip;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.volume = 0f;

            BuildLookupTable();

            currentScene = SceneManager.GetActiveScene().name;
            PlayMusicForScene(currentScene);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void BuildLookupTable()
    {
        musicLookup = new Dictionary<string, AudioClip>();
        foreach (var entry in sceneMusicList)
        {
            if (!musicLookup.ContainsKey(entry.sceneName))
                musicLookup.Add(entry.sceneName, entry.musicClip);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // same scene reloaded → fade in from saved timestamp
        if (scene.name == currentScene)
        {
            StopAllCoroutines();

            if (resumeAfterReload)
            {
                StartCoroutine(FadeInResume());
                resumeAfterReload = false;
            }
            else
            {
                StartCoroutine(FadeInNewTrack());
            }

            return;
        }

        // new scene → change music
        currentScene = scene.name;
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        if (musicLookup.ContainsKey(sceneName))
        {
            StopAllCoroutines();
            StartCoroutine(SwapTrack(musicLookup[sceneName]));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutOnly());
        }
    }

    // Fade-out for switching tracks
    private IEnumerator SwapTrack(AudioClip newClip)
    {
        float t = 0;
        float startVol = audioSource.volume;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.time = 0f;
        audioSource.Play();

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, baseVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = baseVolume;
    }

    // Only fades out (used when scene has no assigned music)
    private IEnumerator FadeOutOnly()
    {
        float t = 0;
        float startVol = audioSource.volume;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
    }

    // NEW — fade in, continue from saved timestamp
    private IEnumerator FadeInResume()
    {
        audioSource.time = savedTime;
        audioSource.Play();

        float t = 0f;
        audioSource.volume = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, baseVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = baseVolume;
    }

    // NEW — fade in from the start of clip (reload with no resume)
    private IEnumerator FadeInNewTrack()
    {
        float t = 0f;
        audioSource.volume = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, baseVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = baseVolume;
    }

    // NEW — called by PlayerDeath
    public void FadeOutAndStoreTime()
    {
        savedTime = audioSource.time; // save playback position
        resumeAfterReload = true;

        StopAllCoroutines();
        StartCoroutine(FadeOutOnly());
    }
}
