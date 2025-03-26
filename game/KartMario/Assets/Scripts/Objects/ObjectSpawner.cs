using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    public GameObject GreenShell;

    // Probablemente esto se pueda hacer mejor
    public readonly List<string> possibleObjects = new List<string>()
    {
        "greenShell"
    };

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

            string selectedObject = possibleObjects.ElementAt(_random.Next(0, possibleObjects.Count));

            print("Ha tocado el objeto: " + selectedObject);
            kart.currentObject = selectedObject;
        }
    }

    public void SpawnObject(string objectName, Vector3 position, KartController kart)
    {
        print("Spawneando...");
        if (IsOwner)
        {
            print("v");
            SpawnObjectServerRpc(objectName, position, kart.transform.forward);
        }
    }

    [ServerRpc]
    private void SpawnObjectServerRpc(string objectName, Vector3 position, Vector3 desiredPosition)
    {
        GameObject spawnedObject = null;
        switch (objectName)
        {
            case "greenShell":
                print("Spawneando green shell");
                spawnedObject = Instantiate(GreenShell, position, Quaternion.identity);
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
                shell.direction.x += 20;
                shell.UseObject();
            }
        }
    }
}
