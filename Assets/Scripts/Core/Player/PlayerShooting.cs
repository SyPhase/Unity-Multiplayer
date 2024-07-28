using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] InputReader inputReader;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] GameObject clientProjectilePrefab;
    [SerializeField] GameObject serverProjectilePrefab;

    [Header("Settings")]
    [SerializeField] float projectileSpeed = 15f;

    bool isFiring = false;

     // Subscribes "HandlePrimary" Method to "Primary" Event
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        inputReader.PrimaryEvent += HandlePrimary;
    }

     // Unsubscribes "HandlePrimary" Method from "Primary" Event
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        inputReader.PrimaryEvent -= HandlePrimary;
    }

    // Is called whenever primary fire input is changed
    void HandlePrimary(bool isFiring)
    {
        this.isFiring = isFiring;
    }

    void Update()
    {
        if (!IsOwner) { return; }

        if (!isFiring) { return; }

        PrimaryServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up); // ServerRpc

        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up); // Local Method
    }

    // Client sends command to Server to spawn projectile
    [ServerRpc]
    void PrimaryServerRpc(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPosition, Quaternion.identity);
        projectileInstance.transform.up = direction;

        PrimaryClientRpc(spawnPosition, direction);
    }

    // Server sends command back to clients to spawn projectile
    [ClientRpc]
    void PrimaryClientRpc(Vector3 spawnPosition, Vector3 direction)
    {
        if (IsOwner) { return; }

        SpawnDummyProjectile(spawnPosition, direction);
    }

    // Client spawns local dummy projectile (purely visual)
    void SpawnDummyProjectile(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPosition, Quaternion.identity);
        projectileInstance.transform.up = direction;
    }
}
