using EasyTransition;
using Injecta;
using System.Collections.Generic;
using UnityEngine;

public class FriendPrefab : MonoBehaviour
{
    [SerializeField]
    private GameObject acceptButton;

    [SerializeField]
    private GameObject rejectButton;

    [SerializeField]
    private GameObject inviteButton;

    [SerializeField]
    private TransitionSettings transitionSettings;

    [Inject]
    private WebsocketSingleton websocket;

    [Inject]
    private LobbyManager lobbyManager;

    private string lobbyCodeToJoin;

    public UserDto friend;

    private CustomSerializer customSerializer;
    private readonly Dictionary<object, object> dict = new Dictionary<object, object>()
    {
        { "messageType", "" },
        { "reject", true },
        { "lobbyCode", LobbyManager.lobbyCode }
    };

    public static List<string> pendingRequests = new();

    private void Start()
    {
        customSerializer = new CustomSerializer(websocket);
        dict.Add("otherUser", friend.id);
    }

    public async void InviteToMatch()
    {
        dict["messageType"] = MessageType.InviteToBattle;
        await customSerializer.SerializeAndSendAsync(dict, true);
    }

    public async void AcceptInvitation()
    {
        bool couldJoin = await lobbyManager.JoinLobbyByCode(lobbyCodeToJoin); 
        if(couldJoin)
        {
            TransitionManager.Instance().Transition(4, transitionSettings, 0);
        }
    }

    public void RejectInvitation()
    {
        Destroy(gameObject);
    }

    public void SetAvailability(bool acceptOrReject, bool invite, string lobbyCodeToJoin)
    {
        acceptButton.SetActive(acceptOrReject);
        rejectButton.SetActive(acceptOrReject);

        if (LobbyManager.lobbyCode != "" && invite)
        {
            inviteButton.SetActive(true);
        }
        else
        {
            inviteButton.SetActive(false);
        }

        this.lobbyCodeToJoin = lobbyCodeToJoin;
    }
}
