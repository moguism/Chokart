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

    private MinimapManager minimap;

    /*[ServerRpc]
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
    }*/

    // PARA PODER MANDARLE UN OBJETO HABRÍA QUE SERIALIZAR
    [ServerRpc]
    protected void InformServerKartCreatedServerRpc(ulong kartId, string playerName, int userId, int characterId, string playerId, bool isHost, ServerRpcParams rpcParams = default)
    {
        GetPositionManager();

        // El servidor agrega el kart a la lista
        KartController kart = _positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart == null)
        {
            print("Agregando");
            (this as KartController).ownerName = playerName;
            (this as KartController).ownerId = userId;
            (this as KartController).characterIndex = characterId;
            (this as KartController).isHost = isHost;
            _positionManager.karts.Add(this as KartController);
            RelayManager.playersIds.Add(playerId);
        }

        ReloadMinimapClientRpc();
    }

    [ClientRpc]
    private void ReloadMinimapClientRpc()
    {
        if(minimap == null)
        {
            minimap = FindFirstObjectByType<MinimapManager>();
        }

        minimap.UpdatePlayerPositions();
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

    [ServerRpc(RequireOwnership = false)]
    public void DispawnKartServerRpc(ulong kartId, ulong kartAggressor)
    {
        KartController kart = FindObjectsByType<KartController>(FindObjectsSortMode.None).FirstOrDefault(k => k.NetworkObjectId == kartId);
        NotifyNewKillClientRpc(kartAggressor);

        if (kart != null)
        {
            DetectCollision.CreateNewFinishKart(_positionManager, kart, _positionManager.karts.Count);

            _positionManager.CheckVictory(kartId);

            kart.NetworkObject.Despawn(true);
            _positionManager.karts.Remove(kart);
            kart = null;
        }
    }

    [ClientRpc(RequireOwnership = false)]
    private void NotifyNewKillClientRpc(ulong kartId)
    {
        try
        {
            KartController kart = FindObjectsByType<KartController>(FindObjectsSortMode.None).FirstOrDefault(k => k.NetworkObjectId == kartId);
            kart.totalKills += 1;
        }
        catch { }
    }

    protected void GetPositionManager()
    {
        if (_positionManager == null)
        {
            _positionManager = GameObject.Find("Triggers").GetComponent<PositionManager>();
        }
    }
}
