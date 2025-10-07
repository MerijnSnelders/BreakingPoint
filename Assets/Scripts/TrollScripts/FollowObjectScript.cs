using UnityEngine;

public class FollowObjectScript : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Vector3 offset;

    private void Start()
    {
        if (target != null)
            offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (target != null)
            transform.position = target.position + offset;
    }
}
