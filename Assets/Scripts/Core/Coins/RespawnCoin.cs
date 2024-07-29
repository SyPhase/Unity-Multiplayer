using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCoin : Coin
{
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

        return coinValue;
    }
}
