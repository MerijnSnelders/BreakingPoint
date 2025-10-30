using UnityEngine;

public class DestroySpikeScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
