using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // 🔊 shared audio source
    [SerializeField] private AudioClip playSound;     // ▶️ sound for Play button
    [SerializeField] private AudioClip exitSound;     // ❌ sound for Exit button
    [SerializeField] private float soundDelay = 0.3f; // optional delay before action

    // 🎮 Play Game
    public void PlayGame()
    {
        if (audioSource && playSound)
            audioSource.PlayOneShot(playSound);

        // Optional: delay scene load slightly to let the sound play first
        Invoke(nameof(LoadGameScene), soundDelay);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    // 🚪 Exit Game
    public void ExitGame()
    {
        if (audioSource && exitSound)
            audioSource.PlayOneShot(exitSound);

        // optional small delay for sound to finish
        Invoke(nameof(QuitGame), soundDelay);
    }

    private void QuitGame()
    {
        Debug.Log("Quit Game"); // Works in editor
        Application.Quit();     // Works in build
    }
}