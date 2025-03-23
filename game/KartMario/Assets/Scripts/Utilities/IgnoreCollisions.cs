using UnityEngine;

public class IgnoreCollisions : MonoBehaviour
{
    public Collider collision1;
    public Collider collision2;

    private void Start()
    {
        Physics.IgnoreCollision(collision1, collision2);
        Physics.IgnoreCollision(collision2, collision1);
    }

    /*private void Update()
    {
        Physics.IgnoreCollision(collision1, collision2);
        Physics.IgnoreCollision(collision2, collision1);
    }*/
}
