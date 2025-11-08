using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour
{
    // This loads your game scene
    public void PlayGame()
    {
        // Replace "Level1" with your actual first scene name
        SceneManager.LoadScene("LevelSelection");
    }

    // This quits the application
    public void ExitGame()
    {
        Debug.Log("Quit Game"); // works in Editor
        Application.Quit();     // only works in build
    }
}
