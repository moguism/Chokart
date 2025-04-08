using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    // Aquí van los distintos objetos (un poco fullero I know xD)
    #region Lista de objetos

    [System.Serializable] // Para que salgan en el inspector
    public class ObjectWithPositionRange
    {
        public string objectName;
        public int minPosition;
        public int maxPosition;
        public GameObject prefab;
    }

    public List<ObjectWithPositionRange> objectSpawnRanges = new List<ObjectWithPositionRange>();

    #endregion

    public static readonly List<BasicObject> objectsSpawned = new List<BasicObject>();
    private readonly System.Random _random = new System.Random();

    [SerializeField]
    private PositionManager positionManager;

    private void OnTriggerEnter(Collider other)
    {
        var parent = other.gameObject.transform.parent;
        if (parent && parent.CompareTag("Kart"))
        {
            KartController kart = parent.GetComponentInChildren<KartController>();
            //print("Ha entrado al trigger el coche " + kart.NetworkObjectId);
            if(kart.currentObject != "")
            {
                return;
            }

            ObjectWithPositionRange selectedObject = GetObjectBasedOnPosition(kart.position);

            print("Ha tocado el objeto: " + selectedObject.objectName);
            kart.currentObject = selectedObject.objectName;
        }
    }

    private ObjectWithPositionRange GetObjectBasedOnPosition(int kartPosition)
    {
        var availableObjects = objectSpawnRanges
            .Where(o => kartPosition >= o.minPosition && kartPosition <= o.maxPosition)
            .ToList();

        int index = _random.Next(0, availableObjects.Count);
        return availableObjects[index];
    }

    public void SpawnObjectServerRpc(string objectName, Vector3 spawnPosition, Vector3 desiredPosition, float ownerId, ServerRpcParams rpcParams = default)
    {
        GameObject spawnedObject = Instantiate(objectSpawnRanges.FirstOrDefault(o => o.objectName == objectName).prefab, spawnPosition, Quaternion.identity);

        if (spawnedObject != null)
        {
            if (spawnedObject.TryGetComponent<NetworkObject>(out var networkObject))
            {
                networkObject.Spawn(); // Para que lo vean todos los clientes

                BasicObject basic = spawnedObject.GetComponentInChildren<BasicObject>();
                basic.owner = ownerId;

                GreenShell shell = spawnedObject.GetComponentInChildren<GreenShell>();

                if (shell != null)
                {
                    shell.direction = desiredPosition;
                    shell.UseObject();
                }
                else
                {
                    SpeedBoost speedBoost = spawnedObject.GetComponentInChildren<SpeedBoost>();

                    if (speedBoost != null)
                    {
                        speedBoost.parent = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == ownerId);
                        speedBoost.UseObject();
                    }
                }

                objectsSpawned.Add(basic);
            }
        }
    }

    public void DespawnObjectServerRpc(ulong id)
    {
        BasicObject basicObject = objectsSpawned.FirstOrDefault(o => o.NetworkObjectId == id);
        RemoveObject(basicObject);
    }

    public void DespawnObjectServerRpc(BasicObject basicObject)
    {
        RemoveObject(basicObject);
    }

    private void RemoveObject(BasicObject basicObject)
    {
        basicObject.networkObject.Despawn(true);
        objectsSpawned.Remove(basicObject);
    }
}
