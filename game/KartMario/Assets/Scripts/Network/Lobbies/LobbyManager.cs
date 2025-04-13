using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static int maxPlayers = 8;

    private Lobby currentLobby;

    private float heartBeatTimer;
    private float lobbyUpdateTimer;

    public static bool isHost = false;

    public static bool gameStarted = false;

    public bool hasRelay = false;

    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (currentLobby != null && !gameStarted)
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
        if (currentLobby != null && !gameStarted)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                lobbyUpdateTimer = 1.1f;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                currentLobby = lobby;

                bool wasHostBefore = isHost;

                if (!wasHostBefore && currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                {
                    isHost = true;
                }

                if (currentLobby.Data["RELAY_CODE"].Value != "0")
                {
                    if (!isHost)
                    {
                        await RelayManager.JoinRelay(currentLobby.Data["RELAY_CODE"].Value);
                        hasRelay = true;
                    }
                }
            }
        }
    }

    public async void CreateLobby(string playerName)
    {
        try
        {
            Player player = CreateNewPlayer(playerName);
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = player,
                Data = new Dictionary<string, DataObject>
                {
                    { "RELAY_CODE", new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby de " + player.Data["PlayerName"].Value, maxPlayers, createLobbyOptions);
            currentLobby = lobby;
            isHost = true;

            Debug.Log("Lobby creada: " + lobby + ". Código: " + lobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async Task<QueryResponse> ListLobbiesAsync(bool show)
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

    public async void JoinLobbyByCode(string input, string playerName)
    {
        try
        {
            Lobby lobby;

            // Si no ha puesto código, se une a la primera que haya disponible
            if (input == null || input == "")
            {
                QueryResponse result = await ListLobbiesAsync(false);

                if (result.Results.Count == 0)
                {
                    Debug.LogWarning("No hay ninguna lobby disponible");
                    return;
                }

                lobby = await JoinLobbyById(result.Results[0].Id, playerName);
            }
            else
            {
                JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
                {
                    Player = CreateNewPlayer(playerName)
                };

                lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(input, joinLobbyByCodeOptions);
            }


            currentLobby = lobby;
            Debug.Log("Unido a la lobby :D");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async Task<Lobby> JoinLobbyById(string id, string playerName)
    {
        JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
        {
            Player = CreateNewPlayer(playerName)
        };

        return await LobbyService.Instance.JoinLobbyByIdAsync(id, joinLobbyByIdOptions);
    }

    public async Task<bool> StartGame()
    {
        if (isHost)
        {
            string relayCode = await RelayManager.CreateRelay();

            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "RELAY_CODE", new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                }
            });

            currentLobby = lobby;

            return true;
        }
        else
        {
            return false;
        }

    }

    public void LeaveLobby()
    {
        KickPlayer(AuthenticationService.Instance.PlayerId);
        currentLobby = null;
        isHost = false;
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

    private Player CreateNewPlayer(string playerName)
    {
        if (playerName == null || playerName == "")
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
                    playerName)
                }
            }
        };
    }
}
