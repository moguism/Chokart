using Injecta;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField]
    private LobbyItem lobbyItemPrefab;

    [SerializeField]
    private Transform lobbyItemParent;

    private bool isRefreshing;
    private bool isJoining;

    public int totalLobbies = 25;

    [Inject]
    private HostManager manager;

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if(isRefreshing)
        {
            return;
        }

        isRefreshing = true;

        try
        {
            var options = new QueryLobbiesOptions();
            options.Count = totalLobbies;

            // Lobbies que tengan más de 0 espacios disponibles y que no estén bloqueadas
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"
                ),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"
                )
            };

            var lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            foreach(Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            foreach(Lobby lobby in lobbies.Results)
            {
                var lobbyInstance = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyInstance.Initialise(this, lobby);
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError(e);
            isRefreshing = false;
            throw;
        }

        isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if(isJoining)
        {
            return;
        }

        isJoining = true;

        try
        {
            var joiningLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await manager.StartClient(joinCode);
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError(e);
            isJoining = false;
            throw;
        }

        isJoining = false;
    }
}
