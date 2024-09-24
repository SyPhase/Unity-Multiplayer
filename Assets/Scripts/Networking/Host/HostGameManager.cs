using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    Allocation allocation;
    string joinCode;
    string lobbyId;
    NetworkServer networkServer;

    const int MaxConnections = 20; // Max Players Connected
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

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false; // Sets lobby to public (visible in lobby list), could be set by UI button "Private Game" to require join code
            lobbyOptions.Data = new System.Collections.Generic.Dictionary<string, DataObject>() // Creates a dictionary to give join code
            {
                {
                    "JoinCode", new DataObject
                    (
                        visibility: DataObject.VisibilityOptions.Member, // Member means only those who join the lobby can get the join code
                        value: joinCode
                    )
                }
            };

            string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Unknown");
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", MaxConnections, lobbyOptions);

            lobbyId = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
            return;
        }

        // Create NetworkServer object (to handle connection requests)
        networkServer = new NetworkServer(NetworkManager.Singleton);

        // Create UserData object to send to server when requesting connection
        UserData userData = new UserData()
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
            userAuthID = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData); // Convert userData to payload (byte array) to send over network
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes; // Set payload (byte array) as connection data, will be sent when connection starts

        // Begin Hosting game
        NetworkManager.Singleton.StartHost();

        // Load next scene
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    IEnumerator HeartBeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);

            yield return delay;
        }
    }
}
