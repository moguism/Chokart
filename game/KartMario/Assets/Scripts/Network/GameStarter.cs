using Injecta;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    private readonly Dictionary<object, object> dict = new Dictionary<object, object>()
    {
        { "messageType", MessageType.GameStarted }
    };

    public List<GameObject> PossiblePrefabs = new List<GameObject>();

    [Inject]
    public WebsocketSingleton websocketSingleton;

    [SerializeField]
    private NetworkManager networkManager;

    [SerializeField]
    private UnityTransport unityTransport;

    [SerializeField]
    private GameObject DefaultPlayerPrefab;

    private CustomSerializer customSerializer;
    
    async void Start()
    {
        customSerializer = new CustomSerializer(websocketSingleton);

        print(websocketSingleton);

        if (WebsocketSingleton.kartModelIndex != -1)
        {
            // ESTA LISTA TIENE QUE SER IDÉNTICA A LA DE "CarSelection", PERO CON LOS PREFABS EN LUGAR DE LOS MODELOS
            networkManager.NetworkConfig.PlayerPrefab = PossiblePrefabs.ElementAt(WebsocketSingleton.kartModelIndex);

            if (websocketSingleton.isHost)
            {
                networkManager.StartHost();
                await customSerializer.Serialize(dict, true);            
            }
        }
        else
        {
            return; 
            // Para testing únicamente (TODO: Borrar después)
            if(!Application.dataPath.Contains("clone"))
            {
                networkManager.StartHost();
                //networkManager.NetworkConfig.PlayerPrefab = PossiblePrefabs.ElementAt(0);
            }
            else
            {
                networkManager.StartClient();
            }
        }
    }

    public void StartClient(string ip)
    {
        print("IP: " + ip);
        if (ip.Equals("::1"))
        {
            ip = "127.0.0.1";
        }
        unityTransport.SetConnectionData(ip, 7777); // El puerto no debería cambiar
        networkManager.StartClient();
    }
}
