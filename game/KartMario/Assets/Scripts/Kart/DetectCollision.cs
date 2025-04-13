using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DetectCollision : NetworkBehaviour
{
    public KartController kart;

    private PositionManager positionManager;
    private ObjectSpawner spawner;
    private SpectateKart spectateKart;

    private void Start()
    {
        positionManager = FindFirstObjectByType<PositionManager>();
        spawner = FindFirstObjectByType<ObjectSpawner>();
        spectateKart = FindFirstObjectByType<SpectateKart>();

        spectateKart.gameObject.SetActive(false);
    }

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
        BasicObject basicObject = ObjectSpawner.objectsSpawned.FirstOrDefault(o => o.NetworkObjectId == objectId);
        KartController kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);

        bool isBomb = false;

        if(basicObject != null)
        {
            if(basicObject.owner == kartId)
            {
                if(basicObject is Bomb)
                {
                    isBomb = true;

                    if ((basicObject as Bomb).exploded == false || kart.lastBombId == objectId)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            if(basicObject is GreenShell)
            {
                float damageOutput = isBomb ? (basicObject as GreenShell).damageOutput : (basicObject as GreenShell).damageOutput;
                NotifyServerAboutChangeServerRpc(kartId, damageOutput);

                if (!isBomb)
                {
                    spawner.DespawnObjectServerRpc(basicObject);
                }
                else
                {
                    kart.lastBombId = objectId;
                }
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

        Debug.LogWarning("La nueva vida es: " + kart.health + ". El id es: " + kart.NetworkObjectId);
        
        // TODO: No hacer desaparecer, sino darlo como Game Over
        if (kart.health <= 0 && LobbyManager.gameStarted)
        {
            kart.chronometer.StopTimer();
            kart.chronometer.timerText.text = "";

            spectateKart.gameObject.SetActive(true);
            spectateKart.kartCamera = kart.kartCamera;

            DispawnKartServerRpc(kart.NetworkObjectId);
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void DispawnKartServerRpc(ulong kartId)
    {
        KartController kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        kart.NetworkObject.Despawn(true);
        positionManager.karts.Remove(kart);
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifyServerAboutChangeServerRpc(ulong kartId, float damage, ServerRpcParams rpcParams = default)
    {
        KartController kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if(!kart.canBeHurt)
        {
            return;
        }

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
