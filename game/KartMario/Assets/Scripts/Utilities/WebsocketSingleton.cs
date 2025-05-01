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

    public static int kartModelIndex = -1;
    public static bool connected = false;

    private VerticalMenu verticalMenu;

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
        // webSocket = new WebSocket(ENVIRONMENT.SOCKET_URL + "?token=" + token, dict);
        webSocket = new WebSocket(ENVIRONMENT.SOCKET_URL + "?token=" + token);

        webSocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            connected = true;
            //SceneManager.LoadScene(4); // La selección de coches: la idea sería que cada vez que el jugador le de a "Jugar" elija su coche y ya después se le empareje
        };

        webSocket.OnError += (e) =>
        {
            Debug.LogError("Error! " + e);
            /* connected = false;
            if (delete)
            {
                SceneManager.LoadScene(1);
            }*/
        };

        webSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            connected = false;
            if (delete)
            {
                PlayerPrefs.DeleteKey("AccessToken");
                PlayerPrefs.Save();
                SceneManager.LoadScene(1);
            }
        };

        webSocket.OnMessage += (bytes) =>
        {
            ProcessMessage(bytes);
        };

        await webSocket.Connect();
    }

    private void ProcessMessage(byte[] bytes)
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

        switch (messageType)
        {
            case MessageType.InviteToBattle:
                if(verticalMenu == null)
                {
                    verticalMenu = FindFirstObjectByType<VerticalMenu>();
                }

                UserDto userWhoInvited = JsonConvert.DeserializeObject<UserDto>(dict["userWhoInvited"].ToString());
                verticalMenu.ConfigureFriendship(verticalMenu.requestsList.transform, userWhoInvited, false, dict["lobbyCode"].ToString());
                break;
        }
    }

    /*private GameObject FindObjectByName(string name, GameObject destination)
    {
        if(destination == null)
        {
            return GameObject.Find(name);
        }

        return destination;
    }*/
}
