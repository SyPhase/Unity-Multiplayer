using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField] bool destroyOnTrigger = false;
    [SerializeField] bool destroyAfterSeconds = false;
    [SerializeField] float secondsToSelfDestruct = 2f;

    void Start()
    {
        if (destroyAfterSeconds) { Destroy(gameObject, 2f); }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (destroyOnTrigger) { Destroy(gameObject); }
    }
}
