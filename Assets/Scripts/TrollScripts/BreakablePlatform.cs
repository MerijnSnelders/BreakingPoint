using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Platform Parts")]
    [SerializeField] private Rigidbody2D leftPart;
    [SerializeField] private Rigidbody2D rightPart;

    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float rotationAngle = 60f;
    [SerializeField] private float launchForce = 3f;
    [SerializeField] private float torqueForce = 10f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip breakClip;
    [SerializeField, Tooltip("If true, slightly randomizes pitch each time")]
    private bool randomizePitch = true;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag(playerTag)) return;
        triggered = true;

        BreakUpward();
    }

    private void BreakUpward()
    {
        // 🎧 Play break sound immediately when platform splits
        if (audioSource != null && breakClip != null)
        {
            audioSource.pitch = randomizePitch ? Random.Range(0.9f, 1.1f) : 1f;
            audioSource.PlayOneShot(breakClip);
        }

        // ✅ Step 1 — rotate instantly upward
        leftPart.transform.Rotate(0f, 0f, -rotationAngle);
        rightPart.transform.Rotate(0f, 0f, rotationAngle);

        // ✅ Step 2 — disable collision so player falls through
        Collider2D leftCol = leftPart.GetComponent<Collider2D>();
        Collider2D rightCol = rightPart.GetComponent<Collider2D>();
        if (leftCol != null) leftCol.enabled = false;
        if (rightCol != null) rightCol.enabled = false;

        // ✅ Step 3 — enable physics and blast upward
        leftPart.bodyType = RigidbodyType2D.Dynamic;
        rightPart.bodyType = RigidbodyType2D.Dynamic;

        leftPart.AddForce((Vector2.up + Vector2.left) * launchForce, ForceMode2D.Impulse);
        rightPart.AddForce((Vector2.up + Vector2.right) * launchForce, ForceMode2D.Impulse);

        // ✅ Step 4 — optional torque spin
        leftPart.AddTorque(torqueForce);
        rightPart.AddTorque(-torqueForce);
    }
}
