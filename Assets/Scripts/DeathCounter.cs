using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathCounter : MonoBehaviour
{
    public static DeathCounter Instance;

    [Header("UI Elements")]
    public Image deathIcon;          // het hoofd-afbeelding
    public TMP_Text deathCountText;  // de "x 3" tekst

    private int deathCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
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
        if (deathCountText != null)
        {
            deathCountText.text = "x " + deathCount;
        }
    }
}
