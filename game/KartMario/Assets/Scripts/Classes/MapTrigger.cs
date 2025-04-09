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
            // Debug.Log("Ha entrado en el trigger  " + parent.name);
            var kart = parent.GetComponentInChildren<KartController>();
            bool alreadyAdded = false;
            bool shouldContinue = true;

            if (kart.triggers.Contains(this))
            {
                kart.triggers.Remove(this);

               
                // Recalcula quién es el próximo trigger
                if (IsOwner || kart.enableAI)
                {
                    MapTrigger lastTrigger = finishLine.triggers.TakeWhile(x => x != this).DefaultIfEmpty(finishLine.triggers[^1]).LastOrDefault();
                    lastTrigger.ChangeIndexAndCalculatePosition(kart);
                    shouldContinue = false;
                }
            }
            else
            {
                alreadyAdded = true;
                kart.triggers.Add(this);
            }

            // Si es la línea de meta, la agrego si no lo he hecho ya
            if (Equals(finishLine) && !alreadyAdded && kart.triggers.Count != 0)
            {
                kart.triggers.Insert(0, this);
                shouldContinue = true;
            }

            if (!shouldContinue)
            {
                return;
            }

            if (IsOwner || kart.enableAI)
            {
                Debug.Log("Trigger IA activado por " + kart + "QUE ES OWNER : " + IsOwner);

                ChangeIndexAndCalculatePosition(kart);
            }
        }
    }

    protected void ChangeIndexAndCalculatePosition(KartController kart)
    {
        int index = finishLine.triggers.IndexOf(this);
        kart.lastTriggerIndex = index;

        CalculateDistanceToNextTrigger(kart);

        if (kart.enableAI && kart.ai != null)
        {
            kart.ai.UpdateDestination(); // usa el índice nuevo para calcular el siguiente
        }

        Debug.Log("Se actualiza lastTriggerIndex para " + kart.name + " a " + index);
    }

    public void CalculateDistanceToNextTrigger(KartController kart)
    {
        if (kart == null)
        {

            return;
        }

        MapTrigger nextTrigger = GetNextTrigger(kart);

        float distance = Vector3.Distance(kart.currentPosition, nextTrigger.transform.position);
        kart.distanceToNextTrigger = distance;

        //print("El coche " + kart.NetworkObjectId + " está en " + kart.currentPosition);
        //print("La distancia del coche " + kart.NetworkObjectId + " es: " + distance);

        if (IsOwner || kart.enableAI)
        {
            NotifyTriggerChangeServerRpc(kart.NetworkObjectId, kart.lastTriggerIndex, distance);
        }
    }

    private MapTrigger GetNextTrigger(KartController kart)
    {
        int nextIndex = 0;
        if (kart.passedThroughFinishLine)
        {
            nextIndex = (kart.lastTriggerIndex + 1) % finishLine.triggers.Count;
        }

        MapTrigger nextTrigger = finishLine.triggers[nextIndex];

        if (kart.enableAI && kart.ai != null)
        {
            print(nextTrigger);
            if (kart.lastTriggerIndex != nextIndex)
            {
                kart.ai.destination = nextTrigger.transform;
            }
        }

        return nextTrigger;
    }

    [ServerRpc]
    private void NotifyTriggerChangeServerRpc(ulong kartId, int newTriggerIndex, float distanceToNextTrigger, ServerRpcParams rpcParams = default)
    {
        // Debug.Log($"ServerRpc coche {kartId} actualizando a index {newTriggerIndex} distancia {distanceToNextTrigger}");
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
