using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;

public class DistorsionObject : BasicObject
{
    [SerializeField]
    private GameObject effectObject;

    public PositionManager positionManager;
    public CustomVideoPlayer glitchEffect;

    public new void UseObject()
    {
        print("Usada distorsión");
        ApplyDistorsionServerRpc(owner);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ApplyDistorsionServerRpc(ulong kartId)
    {
        var availableKarts = positionManager.karts.Where(k => k.NetworkObjectId != kartId).ToList();

        if(availableKarts.Count == 0)
        {
            return;
        }

        System.Random random = new();

        KartController victim = availableKarts[random.Next(0, availableKarts.Count)];

        var parameters = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { victim.OwnerClientId }
            }
        };

        ApplyDistorsionClientRpc(parameters);

        if(IsOwner)
        {
            DespawnOnTimeServerRpc();
        }
    }

    [ClientRpc]
    private void ApplyDistorsionClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if(glitchEffect == null)
        {
            glitchEffect = GameObject.Find("GlitchEffect").GetComponentInChildren<CustomVideoPlayer>();
        }

        GameObject distorsion = Instantiate(effectObject);

        DistorsionEffect effect = distorsion.GetComponentInChildren<DistorsionEffect>();
        effect.glitchEffect = glitchEffect;
        effect.active = true;
    }
}
