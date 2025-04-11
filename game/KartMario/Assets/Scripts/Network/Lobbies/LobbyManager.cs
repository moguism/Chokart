using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static int maxPlayers = 8;

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

    [SerializeField]
    private GameObject startGameButton;

    [SerializeField]
    private GameObject lobbyItemPrefab;

    [SerializeField]
    private GameObject container;

    private Lobby currentLobby;

    private float heartBeatTimer;
    private float lobbyUpdateTimer;

    public static bool isHost = false;

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

                bool wasHostBefore = isHost;

                if(!wasHostBefore && currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                {
                    isHost = true;
                    SetOptionsAvailability(false, true);
                }

                if (currentLobby.Data["RELAY_CODE"].Value != "0")
                {
                    if(!isHost)
                    {
                        RelayManager.JoinRelay(currentLobby.Data["RELAY_CODE"].Value);
                        SceneManager.LoadScene(2);
                    }
                }

                PrintPlayersInLobby();
            }
        }
    }

    private void PrintPlayersInLobby()
    {
        playersList.text = "";

        foreach(Player player in currentLobby.Players)
        {
            print(player);
            playersList.text += "\n" + player.Data["PlayerName"].Value;
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
                    { "RELAY_CODE", new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby de " + player.Data["PlayerName"].Value, maxPlayers, createLobbyOptions);
            currentLobby = lobby;
            isHost = true;
            SetOptionsAvailability(false, true);

            Debug.Log("Lobby creada: " + lobby + ". Código: " + lobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    /*public async void ListLobbiesButton()
    {
        containerCanvas.SetActive(true);
        await ListLobbiesAsync(true);
    }*/

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

            bool alreadyDeletedLobbies = false;

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
                if(show)
                {
                    if(!alreadyDeletedLobbies)
                    {
                        foreach(Transform lobbyTransform in container.transform)
                        {
                            Destroy(lobbyTransform.gameObject);
                        }

                        alreadyDeletedLobbies = true;
                    }

                    GameObject lobbyItem = Instantiate(lobbyItemPrefab, container.transform);
                    //lobbyItem.transform.SetParent(container.transform);

                    LobbyItem item = lobbyItem.GetComponentInChildren<LobbyItem>();
                    item.LobbyName.text = lobby.Name;
                    item.TotalPlayers.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
                    item.LobbyId = lobby.Id;
                    item.LobbyManager = this;
                }
            }

            return queryResponse;

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        return null;
    }

    public void JoinLobbyButton()
    {
        JoinLobbyByCode();
    }

    public async void JoinLobbyByCode(string lobbyId = null)
    {
        try
        {
            Lobby lobby;

            if (lobbyId != null)
            {
                lobby = await JoinLobbyById(lobbyId);
            }
            else
            {
                // Si no ha puesto código, se une a la primera que haya disponible
                if (lobbyCodeInput.text == "" || lobbyCodeInput.text == null)
                {
                    QueryResponse result = await ListLobbiesAsync(false);

                    if (result.Results.Count == 0)
                    {
                        Debug.LogWarning("No hay ninguna lobby disponible");
                        return;
                    }

                    lobby = await JoinLobbyById(result.Results[0].Id);
                }
                else
                {
                    JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
                    {
                        Player = CreateNewPlayer()
                    };

                    lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCodeInput.text, joinLobbyByCodeOptions);
                }
            }

            currentLobby = lobby;
            SetOptionsAvailability(false, true);
            Debug.Log("Unido a la lobby :D");

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
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

    public async void StartGame()
    {
        if(isHost)
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

            SceneManager.LoadScene(2); // La del coche
        }
    }

    public void LeaveLobby()
    {
        KickPlayer(AuthenticationService.Instance.PlayerId);
        SetOptionsAvailability(true, false);
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

    public void SetOptionsAvailability(bool preOptions, bool inOptions)
    {
        preLobbyOptions.SetActive(preOptions);
        afterLobbyOptions.SetActive(inOptions);

        if (isHost)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
    }
}
