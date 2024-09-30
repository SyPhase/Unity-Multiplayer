using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    NetworkManager networkManager;

    Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }

    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        clientIdToAuth[request.ClientNetworkId] = userData.userAuthID;
        authIdToUserData[userData.userAuthID] = userData;

        response.Approved = true; // Approve player connection
        response.CreatePlayerObject = true; // Create player object when connected
    }

    void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    void OnClientDisconnect(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);
            authIdToUserData.Remove(authId);
        }
    }

    public UserData GetUserDataByClientId(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            if (authIdToUserData.TryGetValue(authId, out UserData userData))
            {
                return userData;
            }
        }

        Debug.LogWarning("ERROR: User not found");
        return null;
    }

    public void Dispose()
    {
        if (networkManager == null) { return; }

        networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        networkManager.OnServerStarted -= OnNetworkReady;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;

        if (networkManager.IsListening) { return; }
        networkManager.Shutdown();
    }
}
