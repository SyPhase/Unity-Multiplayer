using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnTrigger : MonoBehaviour
{
    SpawnPoint spawnPoint;
    int inTrigger = 0;

    void Start()
    {
        spawnPoint = GetComponentInChildren<SpawnPoint>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        inTrigger++;

        if (inTrigger > 0)
        {
            spawnPoint.gameObject.SetActive(false);
        }
        else if (inTrigger < 0)
        {
            Debug.LogWarning("ERROR: Spawn triggered " + inTrigger);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        inTrigger--;

        if (inTrigger == 0)
        {
            spawnPoint.gameObject.SetActive(true);
        }
        else if (inTrigger < 0)
        {
            Debug.LogWarning("ERROR: Spawn triggered " + inTrigger);
        }
    }
}
