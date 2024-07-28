using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField] bool destroyOnTrigger = false;
    [SerializeField] float destroyOnTriggerDelay = 0f;
    [SerializeField] bool destroyAfterTime = false;
    [SerializeField] float destroyAfterTimeDelay = 2f;

    void Start()
    {
        if (!destroyAfterTime) { return; }

        Destroy(gameObject, destroyAfterTimeDelay);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!destroyOnTrigger) { return; }

        Destroy(gameObject, destroyOnTriggerDelay);
    }
}
