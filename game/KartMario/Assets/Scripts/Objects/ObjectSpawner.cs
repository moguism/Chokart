using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    #region Prefabs
    // Aquí irían los distintos prefabs
    public GameObject GreenShell;
    #endregion

    // Aquí van los distintos objetos (un poco fullero I know xD)
    #region Lista de objetos

    [System.Serializable] // Para que salgan en el inspector
    public class ObjectWithPositionRange
    {
        public string objectName;
        public int minPosition;
        public int maxPosition;
    }

    public List<ObjectWithPositionRange> objectSpawnRanges = new List<ObjectWithPositionRange>()
    {
        new ObjectWithPositionRange() { objectName = "greenShell", minPosition = 1, maxPosition = 12 },
    };

    #endregion

    public readonly List<BasicObject> objectsSpawned = new List<BasicObject>();
    private readonly System.Random _random = new System.Random();
        
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

            string selectedObject = GetObjectBasedOnPosition(kart.position);

            print("Ha tocado el objeto: " + selectedObject);
            kart.currentObject = selectedObject;
        }
    }

    private string GetObjectBasedOnPosition(int kartPosition)
    {
        var availableObjects = objectSpawnRanges
            .Where(o => kartPosition >= o.minPosition && kartPosition <= o.maxPosition)
            .Select(o => o.objectName)
            .ToList();

        int index = _random.Next(0, availableObjects.Count);
        return availableObjects[index];
    }

    public void SpawnObjectServerRpc(string objectName, Vector3 spawnPosition, Vector3 desiredPosition, float ownerId, ServerRpcParams rpcParams = default)
    {
        GameObject spawnedObject = null;
        switch (objectName)
        {
            case "greenShell":
                print("Spawneando green shell");
                spawnedObject = Instantiate(GreenShell, spawnPosition, Quaternion.identity);
                break;
        }

        if (spawnedObject != null)
        {
            NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Spawn(); // Para que lo vean todos los clientes
                
                GreenShell shell = spawnedObject.GetComponentInChildren<GreenShell>();
                shell.direction = desiredPosition;
                //shell.direction.x += 20;
                shell.owner = ownerId;
                shell.UseObject();
                
                objectsSpawned.Add(shell);
            }
        }
    }

    public void DespawnObjectServerRpc(ulong objectId)
    {
        BasicObject basicObject = objectsSpawned.FirstOrDefault(o => o.NetworkObjectId == objectId);
        if (basicObject != null)
        { 
            basicObject.networkObject.Despawn(true);
        }
    }

    public void DespawnObjectServerRpc(BasicObject basicObject)
    {
        objectsSpawned.Remove(basicObject);
    }
}
