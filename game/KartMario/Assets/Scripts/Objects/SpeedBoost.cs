using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class SpeedBoost : BasicObject
{
    [SerializeField]
    private float boostTimer;

    public KartController parent;
    private float oldSpeed;

    [SerializeField]
    private float speedBoost;

    private PositionManager positionManager;

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
                InformClientAboutChangeClientRpc(parent.NetworkObjectId, parent.acceleration);

                if (IsOwner)
                {
                    DespawnOnTimeServerRpc();
                }
            }

            InformClientAboutChangeClientRpc(parent.NetworkObjectId, parent.acceleration);
        }
    }

    [ClientRpc]
    private void InformClientAboutChangeClientRpc(ulong kartId, float accelaration)
    {
        if(positionManager == null)
        {
            positionManager = FindFirstObjectByType<PositionManager>();
        }

        KartController kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart != null)
        {
            kart.acceleration = accelaration;
        }
    }

    public new void UseObject()
    {
        print("Usado speed boost");
    }
}
