using System.Collections;
using UnityEngine;

public class WallRisingScript : MonoBehaviour
{
    [Header("Walls to Move")]
    [SerializeField] private GameObject[] walls; // One or multiple walls

    [Header("Movement Settings")]
    [SerializeField] private Vector2 moveDirection = Vector2.up; // Direction to move (e.g., up)
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float moveSpeed = 1f; // Controls how fast it reaches in 1 second
    [SerializeField] private bool oneTime = true; // Only trigger once or reusable

    [Header("Optional Audio")]
    [SerializeField] private AudioSource audioSource; // Optional AudioSource
    [SerializeField] private AudioClip riseSoundClip;  // Sound that plays when wall rises
    [SerializeField, Tooltip("Randomize pitch slightly for variation")]
    private bool randomizePitch = true;

    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (oneTime && isActivated) return;

        if (other.CompareTag("Player"))
        {
            isActivated = true;

            // 🎵 Play only 1 second of the sound (if assigned)
            if (audioSource != null && riseSoundClip != null)
                StartCoroutine(PlayOneSecondOfSound());

            // 🧱 Move walls
            foreach (GameObject wall in walls)
            {
                if (wall != null)
                    StartCoroutine(MoveWall(wall.transform));
            }
        }
    }

    private IEnumerator MoveWall(Transform wallTransform)
    {
        Vector3 startPos = wallTransform.position;
        Vector3 targetPos = startPos + (Vector3)(moveDirection.normalized * moveDistance);

        float duration = 1f / moveSpeed; // Movement duration depends on moveSpeed
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            wallTransform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        wallTransform.position = targetPos;
    }

    private IEnumerator PlayOneSecondOfSound()
    {
        if (randomizePitch)
            audioSource.pitch = Random.Range(0.95f, 1.05f);

        audioSource.clip = riseSoundClip;
        audioSource.Play();
        yield return new WaitForSeconds(1f);
        audioSource.Stop(); // ⏹️ Stop after 1 second
    }
}
