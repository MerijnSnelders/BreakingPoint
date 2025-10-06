using UnityEngine;

public class GrowingWallTriggerScript : MonoBehaviour
{
    public GrowingWallScript wall;

    public string playerTag = "Player";

    public float minUpwardVelocity = 0.1f;

    public bool singleUse = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        if (rb.linearVelocity.y <= minUpwardVelocity) return;

        if (other.transform.position.y >= transform.position.y) return;

        if (wall != null) wall.Grow();
        else Debug.LogWarning("JumpTrigger: no TrollWall assigned.");

        if (singleUse)
        {
            gameObject.SetActive(false);
        }
    }
}
