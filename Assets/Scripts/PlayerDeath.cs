using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float fallThreshold = -10f; // Y position below which player dies
    [SerializeField] private LayerMask deadlyLayer; // assign DeadlyObstacle layer in inspector

    private void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.layer);
        if (((1 << collision.gameObject.layer) & deadlyLayer) != 0)
        {
            Die();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & deadlyLayer) != 0)
        {
            Debug.Log("☠️ Player touched deadly (trigger): " + collision.gameObject.name);
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
