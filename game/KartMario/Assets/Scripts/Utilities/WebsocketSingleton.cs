using NativeWebSocket;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WebsocketSingleton : MonoBehaviour
{
    public WebSocket webSocket;

    public bool delete = true;
    public bool isHost = false;

    // Objetos relevantes
    public GameObject managerObject;
    public GameObject gameStarter;

    public static int kartModelIndex = -1;

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
            SceneManager.LoadScene(3); // La selección de coches: la idea sería que cada vez que el jugador le de a "Jugar" elija su coche y ya después se le empareje
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
                managerObject = FindObjectByName("MANAGER", managerObject);
                managerObject.GetComponentInChildren<Lobbies>().HostingComplete(dict["participants"].ToString());
                break;
            case MessageType.PlayerJoined:
                managerObject = FindObjectByName("MANAGER", managerObject);
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
                SceneManager.LoadScene(1); // Esto habrá que cambiarlo, probablemente
                break;
            case MessageType.PlayerDisconnected:
                // Si aún estoy en la lobbie simplemente recargo la lista de jugadores
                managerObject = FindObjectByName("MANAGER", managerObject);
                if (managerObject != null)
                {
                    managerObject.GetComponentInChildren<Lobbies>().JoinedComplete(dict);
                }
                break;
            case MessageType.GameStarted:
                await SceneManager.LoadSceneAsync(2); // Quizás no hace falta hacerlo asíncrono, pero lo hago por si las moscas para que carguen todos los objetos
                gameStarter = FindObjectByName("GameStarter", gameStarter);
                gameStarter.GetComponent<GameStarter>().StartClient(dict["ip"].ToString());
                break;
        }
    }

    private GameObject FindObjectByName(string name, GameObject destination)
    {
        if(destination == null)
        {
            return GameObject.Find(name);
        }

        return destination;
    }
}
