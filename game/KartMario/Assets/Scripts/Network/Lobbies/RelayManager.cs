using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RelayManager : MonoBehaviour
{
    private static RelayServerData _relayServerData;

    public static async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(LobbyManager.maxPlayers);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Código: " + joinCode);

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            _relayServerData = relayServerData;

            //NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_relayServerData);

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

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            _relayServerData = relayServerData;

            print("R1: " + _relayServerData + ". R2:" + relayServerData);

            //NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_relayServerData);

            /*SceneManager.LoadScene(2);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();*/
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
