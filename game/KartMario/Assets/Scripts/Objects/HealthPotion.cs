using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class HealthPotion : BasicObject
{
    [SerializeField]
    private float healthChange;

    public PositionManager positionManager;

    public new void UseObject()
    {
        print("Usando curación");

        if (IsOwner)
        {
            NotifyServerAboutChangeServerRpc(owner);
            DespawnOnTimeServerRpc();
        }
    }

    [ClientRpc]
    private void NotifyHealthChangedClientRpc(ulong kartId, ClientRpcParams clientRpcParams = default)
    {
        KartController kart = FindObjectsByType<KartController>(FindObjectsSortMode.None).FirstOrDefault(k => k.NetworkObjectId == kartId);
        kart.health += healthChange;

        Debug.LogWarning("La nueva vida es: " + kart.health + ". El id es: " + kart.NetworkObjectId);
    }

    [ServerRpc]
    public void NotifyServerAboutChangeServerRpc(ulong kartId, ServerRpcParams rpcParams = default)
    {
        if (positionManager == null)
        {
            positionManager = FindFirstObjectByType<PositionManager>();
        }

        KartController kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);

        var parameters = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { kart.OwnerClientId }
            }
        };

        Debug.LogWarning("OWNER: " + kart.OwnerClientId);

        NotifyHealthChangedClientRpc(kartId, parameters);

    }
}
