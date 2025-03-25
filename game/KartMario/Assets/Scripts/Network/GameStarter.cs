using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    private readonly Dictionary<object, object> dict = new Dictionary<object, object>()
    {
        { "messageType", MessageType.GameStarted }
    };

    async void Start()
    {
        if(Singleton.Instance != null && Singleton.Instance.isHost)
        {
            GetComponentInParent<NetworkManager>().StartHost();
            await CustomSerializer.Serialize(dict, true);
        }
    }

    public void StartClient(string ip)
    {
        print("IP: " + ip);
        if (ip.Equals("::1"))
        {
            ip = "127.0.0.1";
        }
        GetComponentInParent<UnityTransport>().SetConnectionData(ip, 7777); // El puerto no debería cambiar
        GetComponentInParent<NetworkManager>().StartClient();
    }
}
