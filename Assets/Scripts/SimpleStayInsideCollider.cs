using UnityEngine;

public class SimpleStayInsideCollider : MonoBehaviour
{
    public Collider Collider;
    void FixedUpdate()
    {
        if (Collider.ClosestPoint(transform.position) != transform.position)
        {
            transform.position = Collider.ClosestPoint(transform.position);
        }
    }
}
