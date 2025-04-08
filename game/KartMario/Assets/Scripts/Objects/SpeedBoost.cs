using UnityEngine;

public class SpeedBoost : BasicObject
{
    [SerializeField]
    private float boostTimer;

    public KartController parent;
    private float oldSpeed;

    [SerializeField]
    private float speedBoost;

    void Update()
    {
        if(parent != null)
        {
            if (oldSpeed == 0)
            {
                oldSpeed = parent.acceleration;
            }

            boostTimer -= Time.deltaTime;
            parent.acceleration = speedBoost;

            if(boostTimer <= 0.0f)
            {
                parent.acceleration = oldSpeed;

                if (IsOwner)
                {
                    DespawnOnTimeServerRpc(NetworkObjectId);
                }
            }
        }
    }

    public new void UseObject()
    {
        print("Usado speed boost");
    }
}
