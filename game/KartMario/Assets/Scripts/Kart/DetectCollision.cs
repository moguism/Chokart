using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DetectCollision : NetworkBehaviour
{
    public KartController kart;

    // Seguramente haya que cambiar el DamageMultiplier porque la vida se va a quitar dos veces: el que choca (que pide que se quite vida) y el que recibe el choque (que calcula que tiene que restarse vida) 
    private void OnCollisionEnter(Collision collision)
    {
        var parent = collision.gameObject.transform.parent;
        if (parent && parent.CompareTag("Kart"))
        {
            KartController otherKart = parent.GetComponentInChildren<KartController>();
            print("ENTER " + parent.name + ". Salud: " + otherKart.health);

            float kartSpeed = kart.sphere.linearVelocity.magnitude;
            print("VELOCIDAD PROPIA: " + kartSpeed);

            float otherKartSpeed = otherKart.sphere.linearVelocity.magnitude;
            print("VELOCIDAD DEL OPONENTE: " + otherKartSpeed);

            if (kartSpeed > otherKartSpeed)
            {
                //Debug.LogError("a");

                CheckAndRemoveObject(otherKart, kartSpeed);
            }
            else if (otherKartSpeed > kartSpeed)
            {
                //Debug.LogError("b");

                CheckAndRemoveObject(kart, otherKartSpeed);
            }
            else
            {
                //Debug.LogError("c");

                // Quito vida a los dos por igual
                CheckAndRemoveObject(kart, otherKartSpeed);
                CheckAndRemoveObject(otherKart, kartSpeed);
            }
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            kart.isGrounded = true;
        }
    }

    private void CheckAndRemoveObject(KartController kartController, float speed)
    {
        if (IsOwner)
        {
            //Debug.LogError("d");
            /*kartController.health -= damage;
            Debug.LogWarning("La nueva vida es: " + kartController.health + ". El id es: " + kartController.NetworkObjectId);
            if (kartController.health <= 0)
            {
                kartController.gameObject.SetActive(false);
            }
            NotifyHealthChangedClientRpc(damage, kartController.NetworkObjectId);*/

            var damage = speed * BasicPlayer.damageMultiplier;
            NotifyServerAboutChangeServerRpc(kartController.NetworkObjectId, damage);
        }
    }

    [ClientRpc]
    private void NotifyHealthChangedClientRpc(float damage, ulong kartId, ClientRpcParams clientRpcParams = default)
    {
        //Debug.LogError("E");
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
        //Debug.LogError("G");
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

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            kart.isGrounded = false;
        }
    }
}
