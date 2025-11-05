using UnityEngine;
using UnityEngine.UI;

public class DeathCounter : MonoBehaviour
{
    public static DeathCounter Instance;

    [Header("UI (Optional)")]
    public Text deathText; // Assign your UI Text element here in the Inspector

    private int deathCount = 0;

    private void Awake()
    {
        // Ensure this object stays across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            deathCount = PlayerPrefs.GetInt("DeathCount", 0);
            UpdateUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddDeath()
    {
        deathCount++;
        PlayerPrefs.SetInt("DeathCount", deathCount);
        PlayerPrefs.Save();
        UpdateUI();
    }

    public void ResetDeaths()
    {
        deathCount = 0;
        PlayerPrefs.SetInt("DeathCount", 0);
        PlayerPrefs.Save();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (deathText != null)
        {
            deathText.text = "Deaths: " + deathCount;
        }
    }
}