using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DamageOnTrigger : MonoBehaviour
{
    [SerializeField] int damage = 25;

    ulong ownerClientID = 9999;

    public void SetOwner(ulong ownerClientID)
    {
        this.ownerClientID = ownerClientID;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Checks for rigidbody
        if (other.attachedRigidbody == null) { return; }

        // Ignores a tank's own projectiles (may be redundant because of the "IgnoreCollision" in PlayerShooting.cs))
        if (other.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
        {
            if (ownerClientID == networkObject.OwnerClientId) { return; }
        }

        // Finds Health component and deals damage
        if (other.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
