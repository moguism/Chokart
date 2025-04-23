using Injecta;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            if (WebsocketSingleton.kartModelIndex != -1)
            {
                networkManager.NetworkConfig.PlayerPrefab = PossiblePrefabs.ElementAt(WebsocketSingleton.kartModelIndex);
            }
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
                botSpawner.Spawn(spawners[totalSpawned].transform.position, false);
                totalSpawned++;
            }
        }

        await Task.Delay(1000); // Podemos mostrar una pantalla de carga mientras, esto es para que los coches se creen y le de tiempo a notificar de su existencia

        // Una vez que les he cambiado la posición, notifico de empezar la cuenta atrás
        positionManager.InformAboutGameStart();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Cliente {clientId} desconectado.");

        if (clientId == 0 && !LobbyManager.isHost)
        {
            print("El host se ha ido");
            LobbiesSceneManager.showError = true;
            SceneManager.LoadScene(2);
        }
        else 
        {
            if(LobbyManager.isHost && LobbyManager.gameStarted)
            {
                KartController kart = positionManager.karts.FirstOrDefault(k => k.OwnerClientId == clientId);
                DetectCollision.CreateNewFinishKart(positionManager, kart, positionManager.karts.Count);
                positionManager.CheckVictory(kart.NetworkObjectId);
            }
        }
    }
}
