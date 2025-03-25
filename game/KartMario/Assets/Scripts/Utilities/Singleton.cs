using NativeWebSocket;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton : MonoBehaviour
{
    public static Singleton Instance { get; private set; }

    public static readonly string BASE_URL = "https://localhost:7048/";
    public static readonly string API_URL = "https://localhost:7048/api/";
    public static readonly string SOCKET_URL = "wss://localhost:7048/socket/";

    public WebSocket webSocket;
    public bool delete = true;

    public bool isHost = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Para que no se destruya
        }
        else
        {
            Destroy(gameObject);
        }
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
            delete = false;
            await webSocket.Close();
        }
    }

    public async Task ConnectToSocket(string token)
    {
        webSocket = new WebSocket(SOCKET_URL + token);

        webSocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            SceneManager.LoadScene(1);
        };

        webSocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        webSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            if (delete)
            {
                PlayerPrefs.DeleteKey("AccessToken");
                PlayerPrefs.Save();
            }
            SceneManager.LoadScene(0);
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
            MessageType messageType = (MessageType)messageTypeInt;

            bool joined = false;

            switch (messageType)
            {
                case MessageType.HostGame:
                    isHost = true;
                    GameObject.Find("MANAGER").GetComponentInChildren<Lobbies>().HostingComplete(dict["participants"].ToString());
                    break;
                case MessageType.PlayerJoined:
                    GameObject.Find("MANAGER").GetComponentInChildren<Lobbies>().SetObjectsActive(true, false);
                    GameObject.Find("MANAGER").GetComponentInChildren<Lobbies>().JoinedComplete(dict);
                    break;
                case MessageType.JoinGame:
                    Debug.LogWarning("Esperando partida...");
                    break;
                case MessageType.StartGame:
                    joined = bool.Parse(dict["joined"].ToString());
                    if (joined)
                    {
                        SceneManager.LoadScene(2); // La del coche
                    }
                    break;
                case MessageType.EndGame:
                    Debug.LogWarning("El host se ha desconectado");
                    SceneManager.LoadScene(1);
                    break;
                case MessageType.PlayerDisconnected:
                    print("a");
                    // Si aún estoy en la lobbie simplemente recargo la lista de jugadores
                    GameObject manager = GameObject.Find("MANAGER");
                    if(manager != null)
                    {
                        print("b");
                        manager.GetComponentInChildren<Lobbies>().JoinedComplete(dict);
                    }
                    break;
            }
        };

        await webSocket.Connect();
    }
}
