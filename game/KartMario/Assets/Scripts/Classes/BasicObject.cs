using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class BasicObject : NetworkBehaviour
{
    public GameObject owner; // Para que un caparaz�n, por ejemplo, no pueda chocar contra el que lo lanz�

    private void OnCollisionEnter(Collision collision)
    {
        var parent = collision.gameObject.transform.parent;
        if(parent && parent.CompareTag("Kart"))
        {
            KartController kart = parent.GetComponentInChildren<KartController>();
            DoObjectConsequences(kart);
        }
    }

    // ESTOS M�TODOS HAY QUE MODIFICARLOS EN CADA OBJETO
    protected void DoObjectConsequences(KartController kart)
    {
        // Por defecto si se trata del que lanz� el objeto, lo ignora (como se va a hacer Override de este m�todo, pues si es un pl�tano por ejemplo, se ignorar�a esto)
        if(kart.gameObject.Equals(owner))
        {
            return;
        }
    }

    public void UseObject(){}

    // ESTOS M�TODOS YA EST�N EN "DetectCollision", QUIZ�S DEBER�AMOS HACER UNA CLASE GEN�RICA

    [ClientRpc]
    private void NotifyHealthChangedClientRpc(float damage, ulong kartId, ClientRpcParams clientRpcParams = default)
    {
        KartController kart = FindObjectsByType<KartController>(FindObjectsSortMode.None).FirstOrDefault(k => k.NetworkObjectId == kartId);
        kart.health -= damage;
        Debug.LogWarning("La nueva vida es: " + kart.health + ". El id es: " + kart.NetworkObjectId);
        if (kart.health <= 0)
        {
            Destroy(kart.gameObject);
        }

    }

    [ServerRpc]
    public void NotifyServerAboutChangeServerRpc(ulong kartId, float damage, ServerRpcParams rpcParams = default)
    {
        PositionManager positionManager = GameObject.Find("Triggers").GetComponent<PositionManager>();

        KartController kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);

        var parameters = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { kart.OwnerClientId }
            }
        };

        NotifyHealthChangedClientRpc(damage, kartId, parameters);

    }
}
