using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class FinishLine : MapTrigger
{
    public List<MapTrigger> triggers = new List<MapTrigger>();

    public new void OnTriggerEnter(Collider other)
    {
        var parent = other.gameObject.transform.parent;
        if (parent == null || !parent.CompareTag("Kart")) return;

        var kart = parent.GetComponentInChildren<KartController>();

        if (kart == null) return;

        ChangeIndexAndCalculatePosition(kart);

        if (!kart.passedThroughFinishLine)
        {
            kart.passedThroughFinishLine = true;
        }

        // Solo cuenta una vuelta si ha activado todos los triggers antes de cruzar la meta
        if (kart.triggers.Count == triggers.Count)
        {
            kart.triggers.Clear();
            kart.totalLaps++;

            if (IsOwner)
            {
                NotifyLapCompletedServerRpc(kart.NetworkObjectId, kart.totalLaps);
            }
        }
    }

    [ServerRpc]
    private void NotifyLapCompletedServerRpc(ulong kartId, int totalLaps, ServerRpcParams rpcParams = default)
    {
        var kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart != null)
        {
            kart.totalLaps = totalLaps;
        }
    }
}
