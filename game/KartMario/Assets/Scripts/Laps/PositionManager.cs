using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PositionManager : NetworkBehaviour
{
    public List<KartController> karts;
    public FinishLine finishLine;

    void LateUpdate()
    {
        if (!IsHost)
        {
            //print("No soy host");
            return;
        }

        foreach (KartController kart in karts)
        {
            finishLine.CalculateDistanceToNextTrigger(kart);
        }

        // Primero los ordena por las vueltas, luego por el último trigger que hayan visitado y finalmente por la distancia al siguiente trigger
        karts = karts
            .Where(k => k != null)
            .Distinct()
            .OrderByDescending(k => k.totalLaps)
            .ThenByDescending(k => k.lastTriggerIndex)
            .ThenBy(k => k.distanceToNextTrigger)
            .ToList();

        for (int i = 0; i < karts.Count(); i++)
        {
            //print("i vale " + i);
            KartController kart = karts.ElementAt(i);

            //print("EL COCHE " + kart.NetworkObjectId + " ESTÁ EN " + kart.currentPosition);

            int newPosition = i + 1;

            NetworkObject networkObject = kart.gameObject.transform.parent.GetComponent<NetworkObject>();
            ulong ownerClient = networkObject.OwnerClientId;

            // Si es el 0 es el servidor
            if (ownerClient == 0)
            {
                AssignNewPosition(newPosition, kart);
            }
            else
            {
                var parameters = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { kart.OwnerClientId }
                    }
                };

                NotifyPositionChangedClientRpc(newPosition, parameters);
            }
        }

    }

    [ClientRpc]
    private void NotifyPositionChangedClientRpc(int newPosition, ClientRpcParams clientRpcParams = default)
    {
        print("POSICIÓN RECIBIDA");

        var kart = karts.FirstOrDefault();
        if (kart != null)
        {
            print("La nueva posición es: " + newPosition);
            AssignNewPosition(newPosition, kart);

        }
    }

    private void AssignNewPosition(int newPosition, KartController kart)
    {
        kart.position = newPosition;

        if (kart.positionText == null)
        {
            kart.positionText = GameObject.Find("PositionValue").GetComponent<TMP_Text>();
        }

        if (kart.totalLaps >= 1 && !kart.enableAI)
        {
            kart.positionText.text = newPosition.ToString();
        }
    }
}