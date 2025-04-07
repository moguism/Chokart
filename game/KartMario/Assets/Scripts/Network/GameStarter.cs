using Injecta;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    public List<GameObject> PossiblePrefabs = new List<GameObject>();

    [Inject]
    public WebsocketSingleton websocketSingleton;

    [SerializeField]
    private NetworkManager networkManager;

    [SerializeField]
    private UnityTransport unityTransport;

    [SerializeField]
    private GameObject DefaultPlayerPrefab;

    private CustomSerializer customSerializer; // Para mandar mensajes por el socket (por ahora inutil)
    
    void Start()
    {
        customSerializer = new CustomSerializer(websocketSingleton);

        if (WebsocketSingleton.kartModelIndex != -1)
        {
            if (LobbyManager.isHost)
            {
                // ESTA LISTA TIENE QUE SER IDÉNTICA A LA DE "CarSelection", PERO CON LOS PREFABS EN LUGAR DE LOS MODELOS
                networkManager.NetworkConfig.PlayerPrefab = PossiblePrefabs.ElementAt(WebsocketSingleton.kartModelIndex);    
            }
        }

        RelayManager.StartGame();
    }

    public void StartClient(string ip)
    {
        print("IP: " + ip);
        unityTransport.SetConnectionData(ip, 7777); // El puerto no debería cambiar
        networkManager.StartClient();
    }
}
