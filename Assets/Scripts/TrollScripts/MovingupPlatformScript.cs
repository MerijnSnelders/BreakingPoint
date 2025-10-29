using UnityEngine;

public class MovingupPlatformScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform targetPosition; // Where to move (create an empty GameObject above)
    public float moveSpeed = 3f;
    public bool returnAfterDelay = false;
    public float returnDelay = 2f;

    private Vector3 startPos;
    private bool moveUp = false;
    private bool movingBack = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (moveUp && !movingBack)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);
        }

        if (movingBack)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, moveSpeed * Time.deltaTime);
        }
    }

    public void Activate()
    {
        if (!moveUp)
        {
            moveUp = true;
            if (returnAfterDelay)
                Invoke(nameof(StartReturn), returnDelay);
        }
    }

    void StartReturn()
    {
        movingBack = false;
    }
}
