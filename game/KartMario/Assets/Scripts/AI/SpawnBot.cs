using Unity.Netcode;
using UnityEngine;

public class SpawnBot : MonoBehaviour
{
    public GameObject botPrefab;
    private GameObject spawnedObject;
    public Transform spawnPosition;

    public void Spawn()
    {
        spawnedObject = Instantiate(botPrefab, spawnPosition.position, Quaternion.identity);
        if (spawnedObject != null)
        {
            KartController kart = spawnedObject.GetComponentInChildren<KartController>();
            kart.enableAI = true;

            if (spawnedObject.TryGetComponent<NetworkObject>(out var networkObject))
            {
                networkObject.Spawn();
            }
        }
    }
}
