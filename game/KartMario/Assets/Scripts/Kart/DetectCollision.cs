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
                CheckAndRemoveObject(otherKart, kartSpeed);
            }
            else if (otherKartSpeed > kartSpeed)
            {

                CheckAndRemoveObject(kart, otherKartSpeed);
            }
            else
            {
                // Quito vida a los dos por igual
                CheckAndRemoveObject(kart, otherKartSpeed);
                CheckAndRemoveObject(otherKart, kartSpeed);
            }
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            kart.isGrounded = true;
        }
        else if(collision.gameObject.CompareTag("Object"))
        {
            BasicObject basicObject = collision.gameObject.GetComponentInChildren<BasicObject>();
            if(IsOwner)
            {
                CheckCollisionWithObjectServerRpc(kart.NetworkObjectId, basicObject.NetworkObjectId);
            }
        }
    }

    [ServerRpc]
    private void CheckCollisionWithObjectServerRpc(ulong kartId, ulong objectId)
    {
        ObjectSpawner spawner = FindAnyObjectByType<ObjectSpawner>();
        BasicObject basicObject = spawner.objectsSpawned.FirstOrDefault(o => o.NetworkObjectId == objectId);
        if(basicObject && basicObject.owner != kartId)
        {
            if(basicObject is GreenShell)
            {
                NotifyServerAboutChangeServerRpc(kart.NetworkObjectId, (basicObject as GreenShell).damageOutput);
                spawner.DespawnObjectServerRpc(basicObject);
            }
        }
    }

    private void CheckAndRemoveObject(KartController kartController, float speed)
    {
        if (IsOwner)
        {
            var damage = speed * BasicPlayer.damageMultiplier;
            NotifyServerAboutChangeServerRpc(kartController.NetworkObjectId, damage);
        }
    }

    [ClientRpc]
    private void NotifyHealthChangedClientRpc(float damage, ulong kartId, ClientRpcParams clientRpcParams = default)
    {
        KartController kart = FindObjectsByType<KartController>(FindObjectsSortMode.None).FirstOrDefault(k => k.NetworkObjectId == kartId);
        kart.health -= damage;
        kart.healthText.text = "Salud: \n" + kart.health;
        Debug.LogWarning("La nueva vida es: " + kart.health + ". El id es: " + kart.NetworkObjectId);
        
        // TODO: No hacer desaparecer, sino darlo como Game Over
        if (kart.health <= 0)
        {
            kart.GetComponentInParent<NetworkObject>().Despawn(true);
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

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            kart.isGrounded = false;
        }
    }
}
