using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectMenu : MonoBehaviour
{
    [Header("Assign Buttons")]
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;
    public Button backButton;

    [Header("Scene Names")]
    public string level1Scene = "Level1";
    public string level2Scene = "Level2";
    public string level3Scene = "Level3";
    public string mainMenuScene = "MainMenu";

    private void Start()
    {
        SetupButton(level1Button, level1Scene);
        SetupButton(level2Button, level2Scene);
        SetupButton(level3Button, level3Scene);
        SetupButton(backButton, mainMenuScene);

    }

    private void SetupButton(Button button, string sceneName)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SceneManager.LoadScene(sceneName));
    }
}
