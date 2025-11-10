using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private void Start()
    {
        foreach (var levelButton in levelButtons)
        {
            if (levelButton.button != null && !string.IsNullOrEmpty(levelButton.sceneName))
            {
                levelButton.button.onClick.AddListener(() => LoadScene(levelButton.sceneName));
            }
        }
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}