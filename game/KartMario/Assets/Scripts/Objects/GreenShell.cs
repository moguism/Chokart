using UnityEngine;

public class GreenShell : BasicObject
{
    public float damageOutput = 10;
    public float targetTime = 60;
    public Vector3 direction;
    public GameObject parent;
    public float speed = 10;

    private void Update()
    {
        targetTime -= Time.deltaTime;
        if(targetTime <= 0.0f)
        {
            Destroy(parent);
        }
    }

    private void FixedUpdate()
    {
        if (direction != null)
        {
            Vector3 moveDirection = direction.normalized;
            parent.transform.Translate(speed * Time.deltaTime * moveDirection);
        }
    }

    protected new void DoObjectConsequences(KartController kart)
    {
        base.DoObjectConsequences(kart);
        if (IsOwner)
        {
            NotifyServerAboutChangeServerRpc(kart.NetworkObjectId, damageOutput);
        }
    }

    public new void UseObject()
    {
        print("Usado caparazón verde");
    }
}
