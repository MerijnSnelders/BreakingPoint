using UnityEngine;

public class PlatformCrusingScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool hasFallen = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Drop()
    {
        if (hasFallen) return; // prevent double-triggering
        hasFallen = true;

        rb.bodyType = RigidbodyType2D.Dynamic; // enable gravity
        rb.gravityScale = 3f;                  // adjust as needed
    }
}
