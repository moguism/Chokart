using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

// CRÉDITOS: https://www.youtube.com/watch?v=bNCzpKX4frg

public class HostManager : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField]
    private int maxConnections = 8;

    public string JoinCode;
    public string LobbyId;

    
    public async void StartHost()
    {
        Allocation allocation;

        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        }
        catch(Exception e)
        {
            Debug.LogError("Error: " + e);
            throw;
        }

        Debug.Log($"Server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"Server: {allocation.AllocationId}");

        try
        {
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch
        {
            Debug.LogError("Relay ha fallado");
            throw;
        }

        // https://discussions.unity.com/t/how-to-use-relayserverdata/1547792
        var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        try
        {
            var createLobbyOptions = new CreateLobbyOptions();
            createLobbyOptions.IsPrivate = false;
            createLobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                { 
                    "JoinCode", new DataObject
                    (
                        visibility: DataObject.VisibilityOptions.Member,
                        value: JoinCode
                    )
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("My Lobby", maxConnections, createLobbyOptions);
            LobbyId = lobby.Id;
            StartCoroutine(HeartBeatLobbyCoroutine(15));
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError(e);
            throw;
        }
    }

    public async Task StartClient(string joinCode)
    {
        JoinAllocation allocation;

        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            Debug.LogError("Fallo al unirse");
            throw;
        }

        Debug.Log($"Cliente: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"Host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
        Debug.Log($"Cliente: {allocation.AllocationId}");

        var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.GetComponent<UnityTransport>().StartClient();
    }

    // Corrutina para que UGS no desactive la lobby
    private IEnumerator HeartBeatLobbyCoroutine(float waitTimeSeconds)
    {
        var delay = new WaitForSeconds(waitTimeSeconds);
        while(true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(LobbyId);
            yield return delay;
        }
    }

}
