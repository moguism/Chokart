using TMPro;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    public TMP_Text LobbyName;
    public TMP_Text TotalPlayers;
    public string LobbyId;
    public LobbyManager LobbyManager;

    public void JoinButton()
    {
        LobbyManager.JoinLobbyByCode(LobbyId);
    }
}
