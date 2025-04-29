using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    private static RelayServerData _relayServerData;
    public static List<string> playersIds = new();

    public static async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(LobbyManager.maxPlayers);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Código: " + joinCode);

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "wss");

            _relayServerData = relayServerData;

            playersIds.Add(AuthenticationService.Instance.PlayerId);

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }

        return null;
    }

    public static void StartRelay()
    {
        print(_relayServerData);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_relayServerData);
        if (LobbyManager.isHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    public static async Task JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "wss");
            _relayServerData = relayServerData;

            print("R1: " + _relayServerData + ". R2:" + relayServerData);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
