using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class BasicPlayer : NetworkBehaviour
{
    // TODO: Aumentar la salud porque 120 es muy poco si hay muchos jugadores JAJAJAJAAJAJ
    public float health = 0;

    public float maxHealth;
    public static readonly float damageMultiplier = 1; // Al ser estático no se muestra en el editor :(
    
    protected GameStarter starter;
    protected PositionManager _positionManager;
    protected ObjectSpawner objectSpawner;

    [ServerRpc]
    protected void InformServerAboutCharacterChangeServerRpc(ulong kartId, int desiredIndex, ulong ownerId, Vector3 position, ServerRpcParams rpcParams = default)
    {
        if (starter == null)
        {
            starter = FindFirstObjectByType<GameStarter>();
        }

        KartController kart = _positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        kart.GetComponentInParent<NetworkObject>().Despawn(true);

        //NetworkManager.Singleton.NetworkConfig.PlayerPrefab = starter.PossiblePrefabs.ElementAt(desiredIndex);

        GameObject prefab = starter.PossiblePrefabs.ElementAt(desiredIndex);
        GameObject gameObject = Instantiate(prefab, position, Quaternion.identity);

        gameObject.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId);
    }

    // PARA PODER MANDARLE UN OBJETO HABRÍA QUE SERIALIZAR
    [ServerRpc]
    protected void InformServerKartCreatedServerRpc(ulong kartId, string playerName, string playerId, ServerRpcParams rpcParams = default)
    {
        GetPositionManager();

        // El servidor agrega el kart a la lista
        KartController kart = _positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart == null)
        {
            print("Agregando");
            (this as KartController).ownerName = playerName;
            _positionManager.karts.Add(this as KartController);
            RelayManager.playersIds.Add(playerId);
        }
    }

    [ServerRpc]
    protected void InformServerKartStatusServerRpc(ulong kartId, Vector3 currentPositionToUpdate, ServerRpcParams rpcParams = default)
    {
        GetPositionManager();

        KartController kart = _positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart != null)
        {
            print("ME HA LLEGADO MENSAJE DEL COCHE " + NetworkObjectId);
            kart.currentPosition = currentPositionToUpdate;
        }
    }

    [ServerRpc]
    protected void SpawnObjectServerRpc(string currentObject, Vector3 currentPosition, Vector3 destination, ulong kartId)
    {
        if (objectSpawner == null)
        {
            objectSpawner = FindFirstObjectByType<ObjectSpawner>();
        }

        objectSpawner.SpawnObject(currentObject, currentPosition, destination, kartId);
    }

    protected void GetPositionManager()
    {
        if (_positionManager == null)
        {
            _positionManager = GameObject.Find("Triggers").GetComponent<PositionManager>();
        }
    }
}
