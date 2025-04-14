using Unity.Netcode;
using UnityEngine;

public class SpawnBot : MonoBehaviour
{
    public GameObject botPrefab;
    private GameObject spawnedObject;
    public Transform spawnPosition;

    public void SpawnButton()
    {
        Spawn(spawnPosition.position, true);
    }

    public void Spawn(Vector3 position, bool canMove)
    {
        spawnedObject = Instantiate(botPrefab, position, Quaternion.identity);
        if (spawnedObject != null)
        {
            KartController kart = spawnedObject.GetComponentInChildren<KartController>();
            kart.enableAI = true;
            kart.canMove = canMove;

            if (spawnedObject.TryGetComponent<NetworkObject>(out var networkObject))
            {
                networkObject.Spawn();
            }
        }
    }
}
