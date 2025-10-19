using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class C : MonoBehaviour
{
    public Image circleImage;         // assign CircleMask here
    public string nextSceneName;      // scene to load
    public float duration = 1f;       // speed of closing
    public bool startOpen = true;     // if true, starts large and closes in

    private RectTransform circleRect;

    void Start()
    {
        circleRect = circleImage.rectTransform;
        if (startOpen)
            circleRect.localScale = Vector3.one * 10f; // start big
        else
            circleRect.localScale = Vector3.zero;
    }

    public void CloseAndLoad()
    {
        StartCoroutine(Close());
    }

    IEnumerator Close()
    {
        float t = 0;
        Vector3 start = circleRect.localScale;
        Vector3 end = Vector3.zero;

        while (t < duration)
        {
            t += Time.deltaTime;
            circleRect.localScale = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
