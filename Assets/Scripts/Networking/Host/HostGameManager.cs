using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    Allocation allocation;
    string joinCode;

    const int MaxConnections = 20;
    const string GameSceneName = "Game"; // name of main game scene (currently 'Game')

    public async Task StartHostAsync()
    {
        try // Allocate Relay
        {
            allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        try // Get allocated relay's join code
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join Code: " + joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        // Set transport
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(allocation, "udp");
        transport.SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();

        // Load next scene
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }
}
