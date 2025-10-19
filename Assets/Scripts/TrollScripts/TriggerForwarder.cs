using UnityEngine;

public class TriggerForwarder : MonoBehaviour
{
    public SpikeBlockAttack parentBlock; // assign parent with attack script

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentBlock.PlayerEntered();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentBlock.PlayerExited();
        }
    }
}
