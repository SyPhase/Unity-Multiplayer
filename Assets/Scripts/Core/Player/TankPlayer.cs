using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>(); // Network Synced String

    [SerializeField] int ownerCamPriority = 50;

    CinemachineVirtualCamera vCam;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Server set username
            UserData userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            PlayerName.Value = userData.userName;
        }

        if (IsOwner)
        {
            // Owner set higher priority on owned vCam
            vCam = GetComponentInChildren<CinemachineVirtualCamera>();
            vCam.Priority = ownerCamPriority;
        }
    }
}
