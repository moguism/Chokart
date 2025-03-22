using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class MapTrigger : NetworkBehaviour
{
    public static FinishLine finishLine;
    public static PositionManager positionManager;

    private void Start()
    {
        if (finishLine == null)
            finishLine = gameObject.transform.parent.GetComponentInChildren<FinishLine>();

        if (positionManager == null)
            positionManager = gameObject.transform.parent.GetComponent<PositionManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        var parent = other.gameObject.transform.parent;
        if (parent && parent.CompareTag("Kart"))
        {
            var kart = parent.GetComponentInChildren<KartController>();

            if (kart.triggers.Contains(this))
            {
                kart.triggers.Remove(this);
            }
            else
            {
                kart.triggers.Add(this);
            }

            if (IsOwner)
            {
                ChangeIndexAndCalculatePosition(kart);
            }
        }
    }

    protected void ChangeIndexAndCalculatePosition(KartController kart)
    {
        int index = finishLine.triggers.IndexOf(this);
        kart.lastTriggerIndex = index;

        CalculateDistanceToNextTrigger(kart, index);
    }

    public void CalculateDistanceToNextTrigger(KartController kart, int lastTriggerIndex = -1)
    {
        if (kart == null) return;

        int nextIndex = (kart.lastTriggerIndex + 1) % finishLine.triggers.Count;
        MapTrigger nextTrigger = finishLine.triggers[nextIndex];

        float distance = Vector3.Distance(kart.currentPosition, nextTrigger.transform.position);
        kart.distanceToNextTrigger = distance;

        print("El coche " + kart.NetworkObjectId + " está en " + kart.currentPosition);
        print("La distancia es: " + distance);

        if (IsOwner)
        {
            NotifyTriggerChangeServerRpc(kart.NetworkObjectId, lastTriggerIndex, distance);
        }
    }

    [ServerRpc]
    private void NotifyTriggerChangeServerRpc(ulong kartId, int newTriggerIndex, float distanceToNextTrigger, ServerRpcParams rpcParams = default)
    {
        var kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart != null)
        {
            kart.distanceToNextTrigger = distanceToNextTrigger;
            if (newTriggerIndex != -1)
            {
                kart.lastTriggerIndex = newTriggerIndex;
            }
        }
    }
}
