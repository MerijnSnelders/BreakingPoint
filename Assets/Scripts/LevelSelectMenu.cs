using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelectMenu : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public Button button;
        public string sceneName;
    }

    [Header("Level Buttons (assign in Inspector)")]
    public LevelButton[] levelButtons;

    [Header("Back Button (assign in Inspector)")]
    [SerializeField] private Button backButton;
    [SerializeField] private string mainMenuScene = "StartMenu";

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip levelClickSound;  // for level buttons
    [SerializeField] private AudioClip backClickSound;   // for back button
    [SerializeField] private float soundDelay = 0.2f;    // small delay before scene change

    private void Start()
    {
        // 🔊 Attach sounds + listeners for each level button
        foreach (var levelButton in levelButtons)
        {
            if (levelButton.button != null && !string.IsNullOrEmpty(levelButton.sceneName))
            {
                levelButton.button.onClick.AddListener(() =>
                {
                    PlaySound(levelClickSound);
                    StartCoroutine(LoadSceneAfterDelay(levelButton.sceneName));
                });
            }
        }

        // 🔊 Attach sound for back button
        if (backButton != null)
        {
            backButton.onClick.AddListener(() =>
            {
                PlaySound(backClickSound);
                StartCoroutine(LoadSceneAfterDelay(mainMenuScene));
            });
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // small variation
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName)
    {
        yield return new WaitForSeconds(soundDelay);
        SceneManager.LoadScene(sceneName);
    }
}
