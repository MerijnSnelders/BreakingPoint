using Unity.Cinemachine;
using UnityEngine;

public class CamareScript : MonoBehaviour
{
    public Transform player;  // assign your player in the inspector
    public float fixedY = 0f; // the Y position you want the camera to stay at
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = new Vector3(player.position.x, fixedY, transform.position.z);
            // Smoothly interpolate to the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }
    }
}
