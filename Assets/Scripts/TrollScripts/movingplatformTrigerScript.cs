using UnityEngine;

public class movingplatformTrigerScript : MonoBehaviour
{
    public movingplatformScript platform; // sleep je platform hier in de Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            platform.ActivatePlatform();
        }
    }
}
