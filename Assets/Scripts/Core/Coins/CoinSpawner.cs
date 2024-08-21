using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] RespawnCoin coinPrefab;

    [SerializeField] int maxCoins = 51;
    [SerializeField] int coinValue = 1;
    [SerializeField] Vector2 xSpawnRange;
    [SerializeField] Vector2 ySpawnRange;
    [SerializeField] LayerMask layerMask;

    float coinRadius = -1;
    Collider2D[] coinBuffer = new Collider2D[1];

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }

    void SpawnCoin()
    {
        RespawnCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
        //coinInstance.transform.SetParent(transform);

        coinInstance.SetValue(coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();
        coinInstance.OnCollected += HandleCoinCollected;
    }

    void HandleCoinCollected(RespawnCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;

        for (int i = 0; i < 1000; i++) // used while(true) in video
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector2 spawnPoint = new Vector2(x, y);

            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }

        // This shoud never happen, but if there is no space for another coin it will break the infinite loop
        return new Vector2(-10f, -10f);
    }
}
