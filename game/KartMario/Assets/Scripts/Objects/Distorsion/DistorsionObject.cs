using System.Linq;
using Unity.Netcode;
using UnityEngine;

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

        ApplyDistorsionClientRpc(kartId, positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId).isHost);

        if(IsOwner)
        {
            DespawnOnTimeServerRpc();
        }
    }

    [ClientRpc]
    private void ApplyDistorsionClientRpc(ulong atacker, bool atackerIsHost)
    {
        KartController notVictimKart = positionManager.karts.FirstOrDefault(k => k.NetworkBehaviourId == atacker);
        
        // La única manera de que no sea null es que o sea el host, o es el mismo cliente
        if(notVictimKart != null)
        {
            if(LobbyManager.isHost)
            {
                if(atackerIsHost)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

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
