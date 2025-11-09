using UnityEngine;

public class ColliderToggle : MonoBehaviour
{
    [Header("Colliders to Toggle")]
    [Tooltip("The first BoxCollider to make solid/not solid.")]
    public BoxCollider2D collider1;
    [Tooltip("The second BoxCollider to make solid/not solid.")]
    public BoxCollider2D collider2;

    void Start()
    {
        Collider selfCollider = GetComponent<Collider>();
        MakeCollidersNotSolid();
    }

     
    private void OnTriggerEnter2D(Collider2D other)
    {
        MakeCollidersSolid();
    }
    private void MakeCollidersSolid()
    {
        if (collider1 != null)
        {
            collider1.isTrigger = false;
        }

        if (collider2 != null)
        {
            collider2.isTrigger = false;
        }
    }
    private void MakeCollidersNotSolid()
    {
        if (collider1 != null)
        {
            collider1.isTrigger = true;
        }

        if (collider2 != null)
        {
            collider2.isTrigger = true;
        }
    }
}
