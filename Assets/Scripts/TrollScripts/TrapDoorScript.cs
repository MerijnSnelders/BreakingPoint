using System.Collections;
using UnityEngine;

public class TrapDoorScript : MonoBehaviour
{
    [Header("References")]
    public Transform[] leftBlocks;
    public Transform[] rightBlocks;
    public Transform leftLimit;
    public Transform rightLimit;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float returnDelay = 2f;
    public bool autoClose = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField, Tooltip("Playback pitch multiplier (1 = normal, >1 = faster)")]
    private float openSoundPitch = 1.3f; // slightly faster
    [SerializeField, Tooltip("How long to let the sound play before stopping it (seconds)")]
    private float openSoundDuration = 1.0f; // shorten the clip

    private bool isActivated = false;
    private Vector3[] leftStartPositions;
    private Vector3[] rightStartPositions;

    private void Start()
    {
        leftStartPositions = new Vector3[leftBlocks.Length];
        rightStartPositions = new Vector3[rightBlocks.Length];

        for (int i = 0; i < leftBlocks.Length; i++)
            leftStartPositions[i] = leftBlocks[i].position;

        for (int i = 0; i < rightBlocks.Length; i++)
            rightStartPositions[i] = rightBlocks[i].position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActivated && other.CompareTag("Player"))
        {
            isActivated = true;

            // 🎵 Play faster, shorter open sound
            if (audioSource != null && openSound != null)
                StartCoroutine(PlayShortenedSound(openSound, openSoundPitch, openSoundDuration));

            StartCoroutine(OpenTrap());
        }
    }

    private IEnumerator OpenTrap()
    {
        bool leftDone = false, rightDone = false;

        while (!leftDone || !rightDone)
        {
            leftDone = true;
            rightDone = true;

            foreach (Transform block in leftBlocks)
            {
                if (block.position.x > leftLimit.position.x)
                {
                    block.position += Vector3.left * moveSpeed * Time.deltaTime;
                    leftDone = false;
                }
            }

            foreach (Transform block in rightBlocks)
            {
                if (block.position.x < rightLimit.position.x)
                {
                    block.position += Vector3.right * moveSpeed * Time.deltaTime;
                    rightDone = false;
                }
            }

            yield return null;
        }

        if (autoClose)
        {
            yield return new WaitForSeconds(returnDelay);
            StartCoroutine(CloseTrap());
        }
    }

    private IEnumerator CloseTrap()
    {
        bool leftDone = false, rightDone = false;

        while (!leftDone || !rightDone)
        {
            leftDone = true;
            rightDone = true;

            for (int i = 0; i < leftBlocks.Length; i++)
            {
                if (leftBlocks[i].position.x < leftStartPositions[i].x)
                {
                    leftBlocks[i].position += Vector3.right * moveSpeed * Time.deltaTime;
                    leftDone = false;
                }
            }

            for (int i = 0; i < rightBlocks.Length; i++)
            {
                if (rightBlocks[i].position.x > rightStartPositions[i].x)
                {
                    rightBlocks[i].position += Vector3.left * moveSpeed * Time.deltaTime;
                    rightDone = false;
                }
            }

            yield return null;
        }

        isActivated = false;
    }

    // 🎧 Helper coroutine to play faster & shorter sound
    private IEnumerator PlayShortenedSound(AudioClip clip, float pitch, float duration)
    {
        audioSource.pitch = pitch;
        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitForSeconds(duration);
        audioSource.Stop();
        audioSource.pitch = 1f; // reset pitch
    }
}
