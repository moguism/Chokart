using NativeWebSocket;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WebsocketSingleton : MonoBehaviour
{
    public static WebsocketSingleton Instance { get; private set; }

    public WebSocket webSocket;
    public bool delete = true;

    public bool isHost = false;

    // Objetos relevantes
    public GameObject managerObject;
    public GameObject networkManager;

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
        if (webSocket != null)
        {
            webSocket.DispatchMessageQueue();
        }
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
        webSocket = new WebSocket(ENVIRONMENT.SOCKET_URL + token);

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

        webSocket.OnMessage += async (bytes) =>
        {
            await ProcessMessage(bytes);
        };

        await webSocket.Connect();
    }

    private async Task ProcessMessage(byte[] bytes)
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
                FindObjectByName("MANAGER", managerObject);
                managerObject.GetComponentInChildren<Lobbies>().HostingComplete(dict["participants"].ToString());
                break;
            case MessageType.PlayerJoined:
                FindObjectByName("MANAGER", managerObject);
                managerObject.GetComponentInChildren<Lobbies>().SetObjectsActive(true, false);
                managerObject.GetComponentInChildren<Lobbies>().JoinedComplete(dict);
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
                // Si aún estoy en la lobbie simplemente recargo la lista de jugadores
                FindObjectByName("MANAGER", managerObject);
                if (managerObject != null)
                {
                    managerObject.GetComponentInChildren<Lobbies>().JoinedComplete(dict);
                }
                break;
            case MessageType.GameStarted:
                await SceneManager.LoadSceneAsync(2); // Quizás no hace falta hacerlo asíncrono, pero lo hago por si las moscas para que carguen todos los objetos
                FindObjectByName("NetworkManager", networkManager);
                networkManager.GetComponent<GameStarter>().StartClient(dict["ip"].ToString());
                break;
        }
    }

    private void FindObjectByName(string name, GameObject destination)
    {
        if(destination == null)
        {
            destination = GameObject.Find(name);
        }
    }

}
