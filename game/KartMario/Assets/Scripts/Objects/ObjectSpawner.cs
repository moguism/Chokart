using Injecta;
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

    [SerializeField]
    private CustomVideoPlayer glitchEffect;

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
            if(selectedObject == null)
            {
                return;
            }

            print("Ha tocado el objeto: " + selectedObject.objectName);
            kart.currentObject = selectedObject.objectName;
            NotifyAboutNewObjectClientRpc(kart.NetworkObjectId, kart.currentObject);
        }
    }

    [ClientRpc]
    private void NotifyAboutNewObjectClientRpc(ulong kartId, string newObject)
    {
        KartController kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if(kart != null)
        {
            kart.currentObject = newObject;
        }
    }

    private ObjectWithPositionRange GetObjectBasedOnPosition(int kartPosition)
    {
        try
        {
            var availableObjects = objectSpawnRanges
                .Where(o => kartPosition >= o.minPosition && kartPosition <= o.maxPosition)
                .ToList();

            print("Objetos disponibles: " + availableObjects.Count);

            int index = _random.Next(0, availableObjects.Count);
            return availableObjects[index];
        }
        catch
        {
            return null;
        }
    }

    public void SpawnObject(string objectName, Vector3 spawnPosition, Vector3 desiredPosition, ulong ownerId)
    {
        Quaternion rotation = Quaternion.LookRotation(desiredPosition);

        GameObject spawnedObject = Instantiate(objectSpawnRanges.FirstOrDefault(o => o.objectName == objectName).prefab, spawnPosition, rotation);

        bool alreadyAdded = false;

        if (spawnedObject != null)
        {
            KartController kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == ownerId);

            if (spawnedObject.TryGetComponent<NetworkObject>(out var networkObject))
            {
                networkObject.Spawn(); // Para que lo vean todos los clientes

                BasicObject basic = spawnedObject.GetComponentInChildren<BasicObject>();
                basic.owner = ownerId;
                basic.networkObject = networkObject;

                GreenShell shell = spawnedObject.GetComponentInChildren<GreenShell>();

                if (shell != null)
                {
                    Bomb bomb = spawnedObject.GetComponentInChildren<Bomb>();
                    if (bomb != null)
                    {
                        bomb.direction = desiredPosition;
                        bomb.owner = ownerId;
                        bomb.UseObject();

                        objectsSpawned.Add(bomb);
                        alreadyAdded = true;
                    }
                    else
                    {
                        shell.direction = desiredPosition;
                        shell.parent = spawnedObject;
                        shell.UseObject();
                    }
                }
                else
                {
                    SpeedBoost speedBoost = spawnedObject.GetComponentInChildren<SpeedBoost>();

                    if (speedBoost != null)
                    {
                        speedBoost.parent = kart;
                        speedBoost.UseObject();
                    }
                    else
                    {
                        PositionChanger positionChanger = spawnedObject.GetComponentInChildren<PositionChanger>();
                        if(positionChanger != null)
                        {
                            positionChanger.parent = kart;
                            positionChanger.UseObject();
                        }
                        else
                        {
                            HealthPotion healthPotion = spawnedObject.GetComponentInChildren<HealthPotion>();
                            if(healthPotion != null)
                            {
                                healthPotion.owner = ownerId;
                                healthPotion.positionManager = positionManager;
                                healthPotion.UseObject();

                                objectsSpawned.Add(healthPotion);
                                alreadyAdded = true;
                            }
                            else
                            {
                                Invulnerability invulnerability = spawnedObject.GetComponentInChildren<Invulnerability>();
                                if(invulnerability != null)
                                {
                                    invulnerability.parent = kart;
                                    invulnerability.positionManager = positionManager;
                                    invulnerability.UseObject();
                                }
                                else
                                {
                                    Invisibility invisibility = spawnedObject.GetComponentInChildren<Invisibility>();
                                    if(invisibility != null)
                                    {
                                        invisibility.parent = kart;
                                        invisibility.UseObject();
                                    }
                                    else
                                    {
                                        DistorsionObject screenDistorsion = spawnedObject.GetComponentInChildren<DistorsionObject>();
                                        if(screenDistorsion != null)
                                        {
                                            screenDistorsion.owner = ownerId;
                                            screenDistorsion.positionManager = positionManager;
                                            screenDistorsion.glitchEffect = glitchEffect;
                                            screenDistorsion.UseObject();

                                            objectsSpawned.Add(screenDistorsion);
                                            alreadyAdded = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (!alreadyAdded) { objectsSpawned.Add(basic); }
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
