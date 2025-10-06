using System.Collections;
using UnityEngine;

public class GrowingWallScript : MonoBehaviour
{
    public float growBy = 19f;

    public float growDuration = 0.25f;

    [Tooltip("If true the wall stays grown. If false it will shrink back after timeBeforeShrink")]
    public bool stayGrown = true;

    public float timeBeforeShrink = 1f;

    public float shrinkDuration = 0.3f;

    private Vector3 startScale;
    private Vector3 targetScale;
    private Coroutine runningRoutine = null;

    void Start()
    {
        startScale = transform.localScale;
        targetScale = new Vector3(startScale.x + growBy, startScale.y, startScale.z);
    }

    // Call this to start growth
    public void Grow()
    {
        if (runningRoutine != null) StopCoroutine(runningRoutine);
        runningRoutine = StartCoroutine(GrowRoutine());
    }

    private IEnumerator GrowRoutine()
    {
        float t = 0f;
        while (t < growDuration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t / growDuration);
            yield return null;
        }
        transform.localScale = targetScale;

        if (!stayGrown)
        {
            yield return new WaitForSeconds(timeBeforeShrink);
            t = 0f;
            Vector3 from = transform.localScale;
            while (t < shrinkDuration)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(from, startScale, t / shrinkDuration);
                yield return null;
            }
            transform.localScale = startScale;
        }

        runningRoutine = null;
    }
}
