using Injecta;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lobbies : MonoBehaviour
{
    public Canvas playerOptions;
    public Canvas playerList;
    public TMP_Text players;
    public GameObject buttonObject;
    public bool isHost = false;
    private CustomSerializer customSerializer;

    [Inject]
    public WebsocketSingleton websocketSingleton;

    private readonly Dictionary<object, object> dict = new Dictionary<object, object>()
    {
        { "messageType", -1 },
        { "host", "" }
    };

    void Start()
    {
        customSerializer = new CustomSerializer(websocketSingleton);
        print(websocketSingleton);
        playerList.gameObject.SetActive(false);
    }

    public async void HostGame()
    {
        isHost = true;
        dict["messageType"] = MessageType.HostGame;
        await customSerializer.Serialize(dict, true);
    }
    
    // LA IDEA SERÍA QUE TU TE UNAS AL PRIMER JUEGO QUE ESTÉ DISPONIBLE, COMO EN EL MARIO KART
    // El host de esa partida puede invitar a amigos si quiere
    public async void JoinGame()
    {
        isHost = false;
        dict["messageType"] = MessageType.JoinGame;
        dict["host"] = "";
        await customSerializer.Serialize(dict, true);
    }

    public async void StartGame()
    {
        dict["messageType"] = MessageType.StartGame;
        await customSerializer.Serialize(dict, true);
    }

    // Cuando recibe mensaje del socket

    public void HostingComplete(string participant)
    {
        if(!playerList.gameObject.activeSelf)
        {
            players.text = participant;
            SetObjectsActive(true, false);
        }
    }

    public void JoinedComplete(Dictionary<object, object> dict)
    {
        List<string> participants = JsonConvert.DeserializeObject<List<string>>(dict["participants"].ToString());
        players.text = "";

        foreach (string participant in participants)
        {
            players.text += "\n" + participant;
        }
    }

    public void SetObjectsActive(bool playerListBool, bool playerOptionsBool)
    {
        playerList.gameObject.SetActive(playerListBool);
        
        // Para que un cliente normal no pueda empezar la partida
        if(isHost && playerListBool)
        {
            buttonObject.SetActive(true);
        }
        else
        {
            buttonObject.SetActive(false);
        }

        playerOptions.gameObject.SetActive(playerOptionsBool);
    }
}
