using UnityEngine;

public class switchDeathHeight : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDeath deathScript = other.GetComponent<PlayerDeath>();
            if (deathScript != null)
            {
                // ✅ eneble fall-death permanently
                deathScript.isLevel3 = false;


            }
        }
    }
}
