using UnityEngine;
using System.Collections;

public class LeverSwitch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private Transform groundToMove;
    [SerializeField] private AudioSource leverAudioSource; // 🎵 lever flip sound
    [SerializeField] private AudioSource moveAudioSource;  // 🎵 block movement sound
    [SerializeField] private string playerTag = "Player";

    [Header("Settings")]
    [SerializeField] private Vector3 moveOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool canToggleBack = false;
    [SerializeField] private bool oneUseOnly = true;

    [Header("Audio Settings")]
    [SerializeField, Tooltip("Playback pitch multiplier (1 = normal, >1 = faster)")]
    private float moveSoundPitch = 1.2f;
    [SerializeField, Tooltip("Stop sound after this many seconds (0 = play full clip)")]
    private float moveSoundDuration = 0.9f;

    private bool isActivated = false;
    private bool isMoving = false;
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        if (groundToMove != null)
        {
            originalPosition = groundToMove.position;
            targetPosition = originalPosition + moveOffset;
        }

        // make sure sounds don't play automatically
        if (leverAudioSource != null)
            leverAudioSource.playOnAwake = false;

        if (moveAudioSource != null)
        {
            moveAudioSource.loop = false;
            moveAudioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (!isActivated)
            ActivateLever();
        else if (canToggleBack)
            DeactivateLever();
    }

    private void ActivateLever()
    {
        if (isActivated && !canToggleBack) return;
        isActivated = true;

        // 🎬 Play lever animation
        if (leverAnimator != null)
            leverAnimator.SetTrigger("Flip");

        // 🎧 Play lever sound
        if (leverAudioSource != null)
            leverAudioSource.Play();

        // 🧱 Move ground
        if (!isMoving && groundToMove != null)
            StartCoroutine(MoveGround(groundToMove.position, targetPosition));
    }

    private void DeactivateLever()
    {
        if (!canToggleBack) return;
        isActivated = false;

        if (leverAnimator != null)
            leverAnimator.SetTrigger("FlipBack");

        if (leverAudioSource != null)
            leverAudioSource.Play();

        if (!isMoving && groundToMove != null)
            StartCoroutine(MoveGround(groundToMove.position, originalPosition));
    }

    private IEnumerator MoveGround(Vector3 start, Vector3 end)
    {
        isMoving = true;

        // 🎵 Play shortened, faster movement sound
        if (moveAudioSource != null)
            StartCoroutine(PlayShortenedSound(moveAudioSource, moveSoundPitch, moveSoundDuration));

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            groundToMove.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        isMoving = false;
    }

    // 🎧 Helper coroutine for faster & shortened playback
    private IEnumerator PlayShortenedSound(AudioSource source, float pitch, float duration)
    {
        source.pitch = pitch;
        source.Play();
        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
            source.Stop();
        }
        source.pitch = 1f; // reset pitch
    }
}
