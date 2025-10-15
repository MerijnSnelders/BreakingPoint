using UnityEngine;

public class PlatformCrusingScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool hasFallen = false;
    public float gravityScale = 20f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Drop()
    {
        if (hasFallen) return; // prevent double-triggering
        hasFallen = true;

        rb.bodyType = RigidbodyType2D.Dynamic; // enable gravity
        rb.gravityScale = gravityScale;                  // adjust as needed
    }
}
