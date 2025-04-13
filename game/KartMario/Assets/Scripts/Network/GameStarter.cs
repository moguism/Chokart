using Injecta;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameStarter : NetworkBehaviour
{
    public List<GameObject> PossiblePrefabs = new List<GameObject>();

    [Inject]
    public WebsocketSingleton websocketSingleton;

    [SerializeField]
    private NetworkManager networkManager;

    [SerializeField]
    private UnityTransport unityTransport;

    [SerializeField]
    private GameObject startGameButton;

    [SerializeField]
    private PositionManager positionManager;

    [SerializeField]
    private GameObject[] spawners;

    void Start()
    {
        if (LobbyManager.isHost)
        {
            startGameButton.SetActive(true);

            if (WebsocketSingleton.kartModelIndex != -1)
            {
                networkManager.NetworkConfig.PlayerPrefab = PossiblePrefabs.ElementAt(WebsocketSingleton.kartModelIndex);
            }
        }

        RelayManager.StartRelay();
    }

    public void StartGame()
    {
        startGameButton.SetActive(false);
        LobbyManager.gameStarted = true;

        for(int i = 0; i < positionManager.karts.Count; i++)
        {
            KartController kart = positionManager.karts[i];
            Vector3 spawnerPosition = spawners[i].transform.position;

            /*kart.sphere.position = spawnerPosition;
            kart.sphere.transform.position = spawnerPosition;
            kart.transform.position = spawnerPosition;*/

            positionManager.ChangeValuesOfKart(spawnerPosition, kart.NetworkObjectId, 0, 0, new int[0], true);
        }
    }
}
