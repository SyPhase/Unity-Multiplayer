using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FollowCamera : NetworkBehaviour
{
    Transform mainCamera;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        mainCamera = Camera.main.transform;
        mainCamera.SetParent(transform);
        mainCamera.position = new Vector3(0f, 0f, mainCamera.position.z);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        mainCamera.SetParent(null);
        mainCamera.position = new Vector3(0f, 0f, mainCamera.position.z);
    }
}
