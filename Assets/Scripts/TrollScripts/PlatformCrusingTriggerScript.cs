using UnityEngine;

public class PlatformCrusingTriggerScript : MonoBehaviour
{
    public PlatformCrusingScript platform;
    public string playerTag = "Player";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            // Make sure player is BELOW the platform, not above or beside
            if (other.transform.position.y < transform.position.y)
            {
                platform.Drop();
            }
        }
    }
}
