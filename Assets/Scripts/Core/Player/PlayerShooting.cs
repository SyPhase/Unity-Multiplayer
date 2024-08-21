using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] InputReader inputReader;
    [SerializeField] CoinWallet coinWallet;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] GameObject clientProjectilePrefab;
    [SerializeField] GameObject serverProjectilePrefab;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] Collider2D playerCollider;

    [Header("Settings")]
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] float fireDelay = 1f;
    [SerializeField] float muzzleFlashDuration = 0.3f;
    [Tooltip("If zero, fires for free.  If positive number, requires that much money to fire one shot")]
    [SerializeField] int costToFire = 1;

    bool isFiring = false;
    float timer = 0f;
    float muzzleFlashTime = 0f;

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
        // Shows muzzle flash for specified muzzleFlashTime in seconds
        if (muzzleFlashTime > 0)
        {
            muzzleFlashTime -= Time.deltaTime;

            if (muzzleFlashTime < 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) { return; } // Only owner can fire

        if (timer > 0) { timer -= Time.deltaTime; } // Count down timer to fire

        if (!isFiring) { return; } // Only fire if isFiring (Primary is pressed)

        if (timer > 0) { return; } // Only fire every fireDelay number of seconds

        if (coinWallet.TotalCoins.Value < costToFire) { return; } // Only allow firing if enough coins can be spent

        PrimaryServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up); // ServerRpc

        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up); // Local Method
        
        timer = fireDelay; //  reset to current time when fired
    }

    // Client sends command to Server to spawn projectile
    [ServerRpc]
    void PrimaryServerRpc(Vector3 spawnPosition, Vector3 direction)
    {
        if (coinWallet.TotalCoins.Value < costToFire) { return; } // Only allow firing if enough coins can be spent
        coinWallet.SpendCoins(costToFire);

        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPosition, Quaternion.identity);
        projectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>()); // doesn't collider with owner's tank

        // Set's Owner ID on projectile to ignore damage from self (may be redundant because of "IgnoreCollision" just above)
        if (projectileInstance.TryGetComponent<DamageOnTrigger>(out DamageOnTrigger damageOnTrigger))
        {
            damageOnTrigger.SetOwner(OwnerClientId);
        }

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

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
        muzzleFlash.SetActive(true);
        muzzleFlashTime = muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPosition, Quaternion.identity);
        projectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>()); // doesn't collider with owner's tank

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }
}
