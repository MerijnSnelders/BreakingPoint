using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player;
    public float fixedY = 0f;
    public float smoothSpeed = 0.125f;
    public float leftClampX = -8f; // 👈 Adjust this in Inspector

    private bool isFollowing = false;

    void Start()
    {
        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null)
                player = obj.transform;
        }

        if (player == null)
        {
            Debug.LogError("❌ Player not found — add 'Player' tag!");
            return;
        }

        // Start camera at correct position
        Vector3 startPos = new Vector3(leftClampX, fixedY, transform.position.z);
        transform.position = startPos;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Follow smoothly
        Vector3 targetPos = new Vector3(player.position.x, fixedY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed);

        // Prevent camera from moving left beyond clamp
        if (transform.position.x < leftClampX)
            transform.position = new Vector3(leftClampX, transform.position.y, transform.position.z);
    }
}