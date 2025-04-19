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
        Dictionary<string, string> dict = new() 
        {
            { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36" }
        };

        // webSocket = new WebSocket(ENVIRONMENT.SOCKET_URL + "?token=" + token, dict);
        webSocket = new WebSocket(ENVIRONMENT.SOCKET_URL + "?token=" + token, dict);

        webSocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            //SceneManager.LoadScene(4); // La selección de coches: la idea sería que cada vez que el jugador le de a "Jugar" elija su coche y ya después se le empareje
        };

        webSocket.OnError += (e) =>
        {
            Debug.LogError("Error! " + e);
        };

        webSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            if (delete)
            {
                PlayerPrefs.DeleteKey("AccessToken");
                PlayerPrefs.Save();
            }
            SceneManager.LoadScene(1);
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

        /*switch (messageType)
        {
            
        }*/
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
