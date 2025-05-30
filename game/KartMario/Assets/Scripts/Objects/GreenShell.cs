using Unity.Netcode;
using UnityEngine;

public class GreenShell : BasicObject
{
    public float damageOutput = 10;
    public float targetTime;
    public Vector3 direction;
    public GameObject parent;
    public float speed = 10;

    protected virtual void Update()
    {
        if (direction != null)
        {
            targetTime -= Time.deltaTime;
            //print("Queda: " + targetTime);
            if (targetTime <= 0.0f)
            {
                if (IsOwner)
                {
                    DespawnOnTimeServerRpc();
                }
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (direction != null)
        {
            Debug.DrawRay(parent.transform.position, direction.normalized * 2, Color.green);

            parent.transform.Translate(speed * Time.deltaTime * direction.normalized, Space.World);
        }
    }

    public new void UseObject()
    {
        print("Usado caparazón verde");
    }
}
