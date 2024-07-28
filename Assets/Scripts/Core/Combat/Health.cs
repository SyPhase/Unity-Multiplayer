using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    // Note: "field:" is required for public get, private set properties
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    bool isDead = false;

    public Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        // Only done on server, then is synced to clients by NetworkVariable
        if (!IsServer) { return; }

        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }

    void ModifyHealth(int healthChangeValue)
    {
        if (isDead) { return; }

        CurrentHealth.Value = Mathf.Clamp(CurrentHealth.Value + healthChangeValue, 0, MaxHealth);

        if (CurrentHealth.Value == 0)
        {
            OnDie?.Invoke(this); // Note: the "?" is a NULL check
            isDead = true;
        }
    }
}
