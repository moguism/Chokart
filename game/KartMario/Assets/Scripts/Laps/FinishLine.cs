using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class FinishLine : MapTrigger
{
    public List<MapTrigger> triggers = new List<MapTrigger>();

    public new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        var parent = other.gameObject.transform.parent;

        if (parent == null || !parent.CompareTag("Kart")) return;

        var kart = parent.GetComponentInChildren<KartController>();

        if (kart == null) return;

        //ChangeIndexAndCalculatePosition(kart);

        // Solo cuenta una vuelta si ha activado todos los triggers (en orden, que es lo que hace "SequenceEqual") antes de cruzar la meta
        if (kart.triggers.SequenceEqual(triggers.Select(t => t.index).ToList()) ||!kart.passedThroughFinishLine)
        {
            kart.passedThroughFinishLine = true;
            kart.triggers = new List<int>() { 0 };
            kart.totalLaps++;
            Debug.LogWarning("El coche " + kart.NetworkObjectId + " ha dado " + kart.totalLaps + " vueltas");

            if (IsOwner)
            {
                NotifyLapCompletedServerRpc(kart.NetworkObjectId, kart.totalLaps);
                ChangeIndexAndCalculatePosition(kart);
            }
        }
        else
        {
            kart.triggers = new List<int>() { 0 };
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
