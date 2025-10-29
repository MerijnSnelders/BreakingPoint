using UnityEngine;
using System.Collections;

public class LeverSwitch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private Transform groundToMove;
    [SerializeField] private AudioSource audioSource;   // 🎧 Add this
    [SerializeField] private string playerTag = "Player";

    [Header("Settings")]
    [SerializeField] private Vector3 moveOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool canToggleBack = false;
    [SerializeField] private bool oneUseOnly = true;

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

        // 🎧 Play sound
        if (audioSource != null)
            audioSource.Play();

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

        if (audioSource != null)
            audioSource.Play(); // plays same sound when toggling back

        if (!isMoving && groundToMove != null)
            StartCoroutine(MoveGround(groundToMove.position, originalPosition));
    }

    private IEnumerator MoveGround(Vector3 start, Vector3 end)
    {
        isMoving = true;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            groundToMove.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        isMoving = false;
    }
}
