using NativeWebSocket;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PruebaBatallas : MonoBehaviour
{
    WebSocket webSocket;
    public TMP_InputField nameInput;
    public Canvas playerOptions;
    public Canvas playerList;
    public TMP_Text players;

    private readonly Dictionary<object, object> dict = new Dictionary<object, object>()
    {
        { "messageType", -1 }
    };

    void Start()
    {
        playerList.gameObject.SetActive(false);
    }

    public async void HostGame()
    {
        if (webSocket == null)
        {
            await ConnectToSocket();
        }

        dict["messageType"] = MessageType.HostGame;
        var prueba = JsonConvert.SerializeObject(dict);
        await webSocket?.SendText(prueba);
    }
    
    // LA IDEA SERÍA QUE TU TE UNAS AL PRIMER JUEGO QUE ESTÉ DISPONIBLE, COMO EN EL MARIO KART
    // El host de esa partida puede invitar a amigos si quiere
    public async void JoinGame()
    {
        if (webSocket == null)
        {
            await ConnectToSocket();
        }

        dict["messageType"] = MessageType.JoinGame;
        var prueba = JsonConvert.SerializeObject(dict);
        await webSocket?.SendText(prueba);
    }

    private async Task ConnectToSocket()
    {
        string userName = nameInput.text;
        if (userName == null || userName == "")
        {
            Debug.LogError("EL NOMBRE NO PUEDE ESTAR VACÍO");
            return;
        }

        webSocket = new WebSocket(Singleton.SOCKET_URL + userName);

        webSocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        webSocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        webSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        webSocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");
            Debug.Log(bytes);

            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);

            Dictionary<object, object> dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(message);
            Debug.Log("OnMessage! " + message);
            print("Tipo de mensaje: " + dict["messageType"]);

            int messageTypeInt = int.Parse(dict["messageType"].ToString());
            MessageType messageType = (MessageType) messageTypeInt;

            switch (messageType)
            {
                case MessageType.HostGame:
                    playerList.gameObject.SetActive(true);
                    playerOptions.gameObject.SetActive(false);
                    players.text += userName;
                    break;
                case MessageType.PlayerJoined:
                    List<string> participants = JsonConvert.DeserializeObject<List<string>>(dict["participants"].ToString());
                    players.text = "";

                    foreach (string participant in participants)
                    {
                        players.text += "\n" + participant;
                    }
                    break;
                case MessageType.JoinGame:
                    playerList.gameObject.SetActive(true);
                    playerOptions.gameObject.SetActive(false);
                    break;
            }
        };

        await webSocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (webSocket != null)
        {
            webSocket.DispatchMessageQueue();
        }
#endif
    }

    private async void OnApplicationQuit()
    {
        if (webSocket != null)
        {
            await webSocket.Close();
        }
    }
}
