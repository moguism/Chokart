using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class HealthChanger : MonoBehaviour
{
    public PositionManager positionManager;

    [ClientRpc]
    private void NotifyHealthChangedClientRpc(float damage, ulong kartId, ClientRpcParams clientRpcParams = default)
    {
        KartController kart = FindObjectsByType<KartController>(FindObjectsSortMode.None).FirstOrDefault(k => k.NetworkObjectId == kartId);
        kart.health -= damage;

        Debug.LogWarning("La nueva vida es: " + kart.health + ". El id es: " + kart.NetworkObjectId);

        // TODO: No hacer desaparecer, sino darlo como Game Over
        if (kart.health <= 0)
        {
            DispawnKartServerRpc(kart.NetworkObjectId);
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void DispawnKartServerRpc(ulong kartId)
    {
        positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId).GetComponent<NetworkObject>().Despawn(true);
    }

    [ServerRpc]
    public void NotifyServerAboutChangeServerRpc(ulong kartId, float damage, ServerRpcParams rpcParams = default)
    {
        if(positionManager == null)
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

        NotifyHealthChangedClientRpc(damage, kartId, parameters);

    }
}
