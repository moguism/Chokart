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

    [Inject]
    private WebsocketSingleton websocket;

    public UserDto friend;

    private CustomSerializer customSerializer;
    private readonly Dictionary<object, object> dict = new Dictionary<object, object>()
    {
        { "messageType", "" },
        { "reject", true },
        { "lobbyCode", LobbyManager.lobbyCode }
    };

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

    public void SetAvailability(bool acceptOrReject, bool invite)
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
    }
}
