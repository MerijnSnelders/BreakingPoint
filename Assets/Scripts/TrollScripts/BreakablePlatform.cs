using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Platform Parts")]
    [SerializeField] private Rigidbody2D leftPart;
    [SerializeField] private Rigidbody2D rightPart;

    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float rotationAngle = 60f; // how far each half rotates upward
    [SerializeField] private float launchForce = 3f;    // how strong the upward push
    [SerializeField] private float torqueForce = 10f;   // how much spin when flying

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag(playerTag)) return;
        triggered = true;

        BreakUpward();
    }

    private void BreakUpward()
    {
        // ✅ Step 1 — rotate instantly upward
        // (same as before, but flipped)
        leftPart.transform.Rotate(0f, 0f, -rotationAngle);
        rightPart.transform.Rotate(0f, 0f, rotationAngle);

        // ✅ Step 2 — remove collision so player falls
        Collider2D leftCol = leftPart.GetComponent<Collider2D>();
        Collider2D rightCol = rightPart.GetComponent<Collider2D>();
        if (leftCol != null) leftCol.enabled = false;
        if (rightCol != null) rightCol.enabled = false;

        // ✅ Step 3 — enable physics and blast upward
        leftPart.bodyType = RigidbodyType2D.Dynamic;
        rightPart.bodyType = RigidbodyType2D.Dynamic;

        // small upward & outward force
        leftPart.AddForce((Vector2.up + Vector2.left) * launchForce, ForceMode2D.Impulse);
        rightPart.AddForce((Vector2.up + Vector2.right) * launchForce, ForceMode2D.Impulse);

        // optional torque spin
        leftPart.AddTorque(torqueForce);
        rightPart.AddTorque(-torqueForce);
    }
}
