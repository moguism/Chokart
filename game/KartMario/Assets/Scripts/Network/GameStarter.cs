using Injecta;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class GameStarter : NetworkBehaviour
{

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
    private SpawnBot botSpawner;

    [SerializeField]
    private GameObject[] spawners;

    [Inject]
    private LobbyManager lobbyManager;

    void Start()
    {
        if (LobbyManager.isHost)
        {
            startGameButton.SetActive(true);
        }

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        RelayManager.StartRelay();
    }

    public async void StartGame()
    {
        if(!LobbyManager.isHost)
        {
            return;
        }

        startGameButton.SetActive(false);
        OptionsSettings.shouldEnableStartButton = false;

        lobbyManager.StartGame();

        //LobbyManager.gameStarted = true;

        int totalSpawned = 0;

        for(int i = 0; i < positionManager.karts.Count; i++)
        {
            KartController kart = positionManager.karts[i];
            Vector3 spawnerPosition = spawners[i].transform.position;

            positionManager.ChangeValuesOfKart(spawnerPosition, kart.NetworkObjectId, 0, 0, 0, new int[0], true);

            totalSpawned++;
        }

        // Relleno con bots hasta llegar al límite
        if (LobbyManager.spawnBotsWhenStarting)
        {
            while(totalSpawned < LobbyManager.maxPlayers)
            {
                Vector3 position = spawners[totalSpawned].transform.position;
                position.y -= 0.9f;

                botSpawner.Spawn(position, false);
                totalSpawned++;
            }
        }

        await UniTask.WaitForSeconds(1); // Podemos mostrar una pantalla de carga mientras, esto es para que los coches se creen y le de tiempo a notificar de su existencia

        // Una vez que les he cambiado la posición, notifico de empezar la cuenta atrás
        positionManager.InformAboutGameStart();
    }


    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Cliente {clientId} desconectado.");

        if(!LobbyManager.isHost)
        {
            if(clientId == 0 || clientId == 1)
            {
                print("El host se ha ido");
                SceneManager.LoadScene(2);
            }
        }
        else 
        {
            if(LobbyManager.gameStarted && clientId != 0)
            {
                KartController kart = positionManager.karts.FirstOrDefault(k => k.OwnerClientId == clientId);
                DetectCollision.CreateNewFinishKart(positionManager, kart, positionManager.karts.Count);
                positionManager.CheckVictory(kart.NetworkObjectId);
            }
        }

        FindFirstObjectByType<MinimapManager>().UpdatePlayerPositions();
    }
}
