using System.Linq;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PositionChanger : BasicObject
{
    public KartController parent;
    private static PositionManager positionManager;
    private readonly System.Random random = new System.Random();

    public new void UseObject()
    {
        print("Usado intercambio de posición");

        if(IsOwner)
        {
            ChangePositionsWithKartServerRpc(parent.NetworkObjectId);
            DespawnOnTimeServerRpc();
        }
    }

    [ServerRpc]
    private void ChangePositionsWithKartServerRpc(ulong originalKartId)
    {
        if (positionManager == null)
        {
            positionManager = FindFirstObjectByType<PositionManager>();
        }

        var availableKarts = positionManager.karts.Where(k => k.NetworkObjectId != originalKartId).ToList();
        if (availableKarts.Count > 0)
        {
            var kart = availableKarts[random.Next(0, availableKarts.Count)];

            Vector3 newPositionToParent = kart.sphere.transform.position;
            Vector3 newPositionToOtherKart = parent.sphere.transform.position;

            int newPositionInRaceForParent = kart.position;
            int newPositionInRaceForOtherKart = parent.position;

            int lastTriggerIndexForParent = kart.lastTriggerIndex;
            int lastTriggerIndexForOtherKart = parent.lastTriggerIndex;

            var triggerListForParent = kart.triggers.ToArray();
            var triggerListForOther = parent.triggers.ToArray();

            positionManager.ChangeValuesOfKart(newPositionToParent, parent.NetworkObjectId, lastTriggerIndexForParent, newPositionInRaceForParent, triggerListForParent);
            positionManager.ChangeValuesOfKart(newPositionToOtherKart, kart.NetworkObjectId, lastTriggerIndexForOtherKart, newPositionInRaceForOtherKart, triggerListForOther);
        }
    }

    /*[ClientRpc]
    private void InformClientsAboutChangeClientRpc(Vector3 newPosition, ulong kartId, int lastTriggerIndex, int position, int[] triggers)
    {
        if(positionManager == null)
        {
            positionManager = FindFirstObjectByType<PositionManager>();
        }

        var kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart != null)
        {
            kart.sphere.position = newPosition;
            kart.sphere.transform.position = newPosition;
            kart.transform.position = newPosition;
            kart.position = position;
            kart.lastTriggerIndex = lastTriggerIndex;
            kart.triggers = triggers.ToList();
        }
    }*/
}
