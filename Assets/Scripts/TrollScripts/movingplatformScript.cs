using System.Collections;
using UnityEngine;

public class movingplatformScript : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;
    public Transform pointD; // eindpunt na heen-en-weer beweging

    [Header("Settings")]
    public float speed = 2f;
    public int repeatCount = 6;
    public float upwardDistance = 3f;

    [Header("Activation")]
    public bool startOnTrigger = true; // alleen starten bij trigger
    private bool isActivated = false;

    private bool trolling = false;
    private Transform platform;
    private Coroutine moveRoutine;

    void Start()
    {
        platform = transform;
    }

    // --- Trigger wordt geactiveerd door een apart object ---
    public void ActivatePlatform()
    {
        if (!isActivated)
        {
            isActivated = true;
            moveRoutine = StartCoroutine(MovePlatform());
        }
    }

    private IEnumerator MovePlatform()
    {
        Vector3 startPoint = pointA.position;
        Vector3 midPoint = pointB.position;
        Vector3 midPoint2 = pointC.position;
        Vector3 endPoint = pointD.position;
        int currentRepeat = 0;

        // Heen-en-weer beweging


        // Naar eindpunt
        yield return MoveBetweenPoints(startPoint, midPoint);

        speed = 15;

        yield return MoveBetweenPoints(midPoint, endPoint);

        while (currentRepeat < repeatCount)
        {
            trolling = true;
            yield return MoveBetweenPoints(endPoint, midPoint2);
            yield return MoveBetweenPoints(midPoint2, endPoint);
            currentRepeat++;
        }
        trolling = false;
        // Omhoog bewegen
        yield return MoveUpwards(upwardDistance);
    }

    private IEnumerator MoveBetweenPoints(Vector3 from, Vector3 to)
    {
        float t = 0f;
        if (trolling)
        {
            speed = 6;
        }
        while (t < 1f)
        {
            t += Time.deltaTime * speed / Vector3.Distance(from, to);
            platform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }
    }

    private IEnumerator MoveUpwards(float distance)
    {
        Vector3 startPos = platform.position;
        Vector3 targetPos = startPos + Vector3.up * distance;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed / distance;
            platform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }

    // --- Belangrijk: speler meebewegen ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
        }
    }
}


