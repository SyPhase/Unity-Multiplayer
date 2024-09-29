using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [SerializeField] int ownerCamPriority = 50;

    CinemachineVirtualCamera vCam;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            vCam = GetComponentInChildren<CinemachineVirtualCamera>();

            vCam.Priority = ownerCamPriority;
        }
    }
}
