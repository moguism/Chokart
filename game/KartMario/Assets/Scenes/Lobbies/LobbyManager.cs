using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    private string lobbyName;

    [SerializeField]
    private int maxPlayers;

    [SerializeField]
    private TMP_InputField lobbyCodeInput;

    [SerializeField]
    private TMP_InputField playerNameInput;

    [SerializeField]
    private TMP_Text playersList;

    [SerializeField]
    private GameObject preLobbyOptions;

    [SerializeField]
    private GameObject afterLobbyOptions;

    private Lobby currentLobby;

    private float heartBeatTimer;
    private float lobbyUpdateTimer;

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (currentLobby != null)
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer < 0f)
            {
                heartBeatTimer = 15;
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (currentLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                lobbyUpdateTimer = 1.1f;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                currentLobby = lobby;
                PrintPlayersInLobby();
            }
        }
    }

    private void PrintPlayersInLobby()
    {
        playersList.text = "";

        foreach(Player player in currentLobby.Players)
        {
            playersList.text += "\n" + player.Data["PlayerName"].Value;
        }
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateLobby()
    {
        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = CreateNewPlayer()
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            currentLobby = lobby;
            SetOptionsAvailability(false, true);

            Debug.Log("Lobby creada: " + lobby + ". Código: " + lobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void ListLobbiesButton()
    {
        await ListLobbiesAsync();
    }

    public async Task<QueryResponse> ListLobbiesAsync()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions();
            queryLobbiesOptions.Filters = new List<QueryFilter>()
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                new QueryFilter(QueryFilter.FieldOptions.IsLocked, "0", QueryFilter.OpOptions.EQ)
            };
            queryLobbiesOptions.Order = new List<QueryOrder>()
            {
                new QueryOrder(false, QueryOrder.FieldOptions.Created)
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies encontradas: " + queryResponse.Results.Count);

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }

            return queryResponse;

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        return null;
    }

    public async void JoinLobbyByCode()
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = CreateNewPlayer()
            };

            string code = lobbyCodeInput.text;

            // Si no ha puesto código, se une a la primera que haya disponible
            if (code == "" || lobbyCodeInput.text == null)
            {
                QueryResponse result = await ListLobbiesAsync();

                if(result.Results.Count == 0)
                {
                    Debug.LogWarning("No hay ninguna lobby disponible");
                    return;
                }
                else
                {
                    code = result.Results[0].LobbyCode;
                }
            }

            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCodeInput.text, joinLobbyByCodeOptions);
            currentLobby = lobby;
            SetOptionsAvailability(false, true);
            Debug.Log("Unido a la lobby :D");

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public void LeaveLobby()
    {
        KickPlayer(AuthenticationService.Instance.PlayerId);
        SetOptionsAvailability(true, false);
        currentLobby = null;
    }

    private async void KickPlayer(string playerId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private Player CreateNewPlayer()
    {
        if(playerNameInput.text == "" || playerNameInput.text == null)
        {
            Debug.LogWarning("Debe de haber un nombre");
            return null;
        }

        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>()
            {
                { "PlayerName", new PlayerDataObject(
                    PlayerDataObject.VisibilityOptions.Member,
                    playerNameInput.text)
                }
            }
        };
    }

    private void SetOptionsAvailability(bool preOptions, bool inOptions)
    {
        preLobbyOptions.SetActive(preOptions);
        afterLobbyOptions.SetActive(inOptions);
    }
}
