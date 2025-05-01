using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static int maxPlayers = 8;

    private Lobby currentLobby;

    private float heartBeatTimer;
    private float lobbyUpdateTimer = 2.0f;

    public static bool isHost = false;
    public static bool gameStarted = false;
    public static bool spawnBotsWhenStarting = false;

    public bool hasRelay = false;
    public static string lobbyCode = "";

    public static string PlayerName;
    public static int PlayerId;

    public static Gamemodes gamemode = Gamemodes.Race;

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
        if (isHost && currentLobby != null && !gameStarted)
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer < 0f)
            {
                heartBeatTimer = 15;
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
        }
        else if(gameStarted)
        {
            currentLobby = null;
        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        try
        {
            if (currentLobby != null && !gameStarted)
            {
                lobbyUpdateTimer -= Time.deltaTime;
                if (lobbyUpdateTimer < 0f)
                {
                    lobbyUpdateTimer = 2f;

                    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                    currentLobby = lobby;

                    bool wasHostBefore = isHost;

                    if (!wasHostBefore && currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                    {
                        isHost = true;
                    }

                    Enum.TryParse(currentLobby.Data["GAMEMODE"].Value, out gamemode);

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
            else
            {
                currentLobby = null;
            }
        } catch(Exception e)
        {
            // Si tira excepción es que me han echado de la lobby
            Debug.LogError(e);

            LobbiesSceneManager.showError = true;
            LeaveLobby(true);

            SceneManager.LoadScene(2);
        }
    }

    public async void CreateLobby()
    {
        try
        {
            Player player = CreateNewPlayer();
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = player,
                Data = new Dictionary<string, DataObject>
                {
                    { "RELAY_CODE", new DataObject(DataObject.VisibilityOptions.Member, "0") },
                    { "GAMEMODE", new DataObject(DataObject.VisibilityOptions.Public, gamemode.ToString()) }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby de " + player.Data["PlayerName"].Value, maxPlayers, createLobbyOptions);
            currentLobby = lobby;
            isHost = true;

            Debug.Log("Lobby creada: " + lobby + ". Código: " + lobby.LobbyCode);

            lobbyCode = lobby.LobbyCode;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
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

    public async Task<bool> JoinLobbyByCode(string input)
    {
        try
        {
            Lobby lobby;

            // Si no ha puesto código, se une a la primera que haya disponible
            if (input == null || input == "")
            {
                QueryResponse result = await ListLobbiesAsync();

                if (result.Results.Count == 0)
                {
                    Debug.LogWarning("No hay ninguna lobby disponible");
                    return false;
                }

                lobby = await JoinLobbyById(result.Results[0].Id);
            }
            else
            {
                JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
                {
                    Player = CreateNewPlayer()
                };

                lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(input, joinLobbyByCodeOptions);
            }

            currentLobby = lobby;
            lobbyCode = currentLobby.LobbyCode;

            Debug.Log("Unido a la lobby :D");

            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    private async Task<Lobby> JoinLobbyById(string id)
    {
        JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
        {
            Player = CreateNewPlayer()
        };

        return await LobbyService.Instance.JoinLobbyByIdAsync(id, joinLobbyByIdOptions);
    }

    public async Task<bool> StartRelay()
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

    public async void StartGame()
    {
        // Expulso de la lobby a todos aquellos que no estén ya en la partida
        for(int i = 0; i < currentLobby.Players.Count; i++)
        {
            Player player = currentLobby.Players[i];
            if(!RelayManager.playersIds.Contains(player.Id))
            {
                await KickPlayer(player.Id);
            }
        }

        Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
        {
            IsLocked = true
        });

        currentLobby = lobby;
        gameStarted = true;
    }

    public async void LeaveLobby(bool alreadyOut)
    {
        currentLobby = null;
        isHost = false;
        gameStarted = false;
        hasRelay = false;

        if(!alreadyOut) 
        { 
            await KickPlayer(AuthenticationService.Instance.PlayerId);
        }
    }

    private async Task KickPlayer(string playerId)
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
        if (PlayerName == null || PlayerName == "")
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
                    PlayerName)
                }
            }
        };
    }
}
