using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCoin : Coin
{
    public event Action<RespawnCoin> OnCollected;

    Vector3 previousPosition;

    void Update()
    {
        if (previousPosition != transform.position)
        {
            Show(true);
        }

        previousPosition = transform.position;
    }

    public override int Collect()
    {
        // if not server, hide coin when collected
        if (!IsServer)
        {
            Show(false);
            return 0;
        }
        
        // Coin cannot be collected more than once
        if (alreadyCollected) { return 0; }
        alreadyCollected = true;

        OnCollected?.Invoke(this);

        return coinValue;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
