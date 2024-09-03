using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FollowCamera : NetworkBehaviour
{
    void Update()
    {
        if (IsOwner) { return; }
        Destroy(gameObject); // Destory this and camera on other players prefabs
    }
}
