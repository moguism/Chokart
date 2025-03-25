using System.Collections.Generic;
using Unity.Netcode;
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
}
